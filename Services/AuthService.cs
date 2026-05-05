using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Hangfire;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using WebApplication1.Common.Constants;
using WebApplication1.Common.Exceptions;
using WebApplication1.Common.Parsers;
using WebApplication1.DTOs;
using WebApplication1.Entities;
using WebApplication1.Interfaces;
using WebApplication1.Jobs;
using WebApplication1.Mappers;
using WebApplication1.Settings;

namespace WebApplication1.Services
{
    public class AuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IRoleRepository _roleRepo;
        private readonly IRefreshTokenRepository _refreshTokenRepo;
        private readonly ILogger<AuthService> _logger;
        private readonly RedisService _redisServcie;
        private readonly AuthSettings _authSettings;
        private readonly RoleSettings _roleSettings;
        private readonly AuthMapper _mapper;
        private readonly PasswordHasher<User> _hasher;
       
        public AuthService(IUserRepository userRepo, IRoleRepository roleRepo,
            IRefreshTokenRepository refreshTokenRepo, ILogger<AuthService> logger, RedisService redisServcie,
            IOptions<AuthSettings> authOptions, IOptions<RoleSettings> roleOptions)
        {
            _userRepo = userRepo;
            _roleRepo = roleRepo;
            _refreshTokenRepo = refreshTokenRepo;
            _logger = logger;
            _redisServcie = redisServcie;
            _authSettings = authOptions.Value;
            _roleSettings = roleOptions.Value;
            _mapper = new AuthMapper();
            _hasher = new PasswordHasher<User>();
        }

        private async Task ValidatePassword(User user, string password)
        {
            if (user.PasswordHash == null) throw new BadRequestException("Invalid email or password");

            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (result == PasswordVerificationResult.Failed)
            {
                throw new BadRequestException("Invalid email or password");
            }
            if (result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.PasswordHash = _hasher.HashPassword(user, password);
                await _userRepo.UpdateAsync(user);
            }
        }

        private string GenerateToken(User user, string roleName)
        {
            var secretKey = _authSettings.Key;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, roleName),
                new Claim(ClaimTypes.Version, user.TokenVersion.ToString() ?? "0")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(_authSettings.ExpiryHours),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
       
        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            return Convert.ToBase64String(randomBytes);
        }

        private static string HashToken(string token)
        {
            using var sha = SHA256.Create();

            var bytes = Encoding.UTF8.GetBytes(token);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private async Task<RefreshToken> CheckRefreshTokenIsValid(string token)
        {
            if (await _redisServcie.ExistAsync("revoked", token))
                throw new BadRequestException("Invalid refresh token");

            var hashedToken = HashToken(token);
            var storedToken = await _refreshTokenRepo.GetByTokenAsync(hashedToken);

            if (storedToken == null)
            {
                await _redisServcie.SetAsync("revoked", token, "1", TimeSpan.FromMinutes(5));
                throw new BadRequestException("Invalid refresh token");
            }

            if(storedToken.ExpiryDate < DateTime.UtcNow)
                throw new BadRequestException("Invalid refresh token");

            if (storedToken.IsRevoked)
            {
                await RevokeAllTokensOfUser(storedToken.UserId);
                throw new ForbiddenException("Token reuse detected");
            }
            return storedToken;
        }

        public async Task<AuthResponseDto> IssueTokens(User user, string roleName, Guid? tokenId = null)
        {
            var accessToken = GenerateToken(user, roleName);
            var refreshToken = GenerateRefreshToken();

            var entity = _mapper.ToRefreshTokenEntity(refreshToken, user.UserId);

            if (tokenId != null)
            {
                entity.ReplacedByTokenId = tokenId.Value;
            }

            entity.Token = HashToken(refreshToken);

            await _refreshTokenRepo.AddAsync(entity);

            return _mapper.ToDto(accessToken, refreshToken);
        }

        public async Task CheckEmailUnique(string email, Guid? excludeId = null)
        {
            var exists = await _userRepo.EmailExistsAsync(email, excludeId);

            if (exists)
                throw new BadRequestException($"Invalid Email '{email}'.");
        }

        public async Task RevokeAllTokensOfUser(Guid userId)
        {
            await _refreshTokenRepo.UpdateRangeAsync(userId);
        }

        public async Task<User?> CheckUserExistAndGetById(Guid userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            return user;
        }

        public void BackgroundRegisterWork(Guid userId)
        {
            //BackgroundJob.Enqueue<AuthBackgroundJobs>(job =>
            //   job.SendRegisterEmailAsync(userId)
            //);
        }

        public void BackgroundLoginWork(Guid userId, UserInfo userInfo)
        {
            BackgroundJob.Enqueue<AuthBackgroundJobs>(job =>
                job.UserLoginHistroyAsync(userId, userInfo)
            );

            //BackgroundJob.Enqueue<AuthBackgroundJobs>(job =>
            //    job.SendLoginAlertEmailAsync(userId, userInfo)
            //);
        }

        public async Task<User?> CheckUserExistAndGetByEmail(string email)
        {
            var user = await _userRepo.GetByEmailAsync(email);
            return user;
        }

        public async Task<Role?> GetUserRole()
        {
           var role = await _roleRepo.GetByNameAsync(_roleSettings.User);
            return role;
        }

        public async Task AddUser(User user)
        {
            await _userRepo.AddAsync(user);
        }

        public async Task<AuthResponseDto> RegisterAsync(AuthRegisterDto dto)
        {
            await CheckEmailUnique(dto.Email);

            var role = await GetUserRole();

            var user = _mapper.ToEntity(dto, role);

            await AddUser(user);

            BackgroundRegisterWork(user.UserId);

            return await IssueTokens(user, role.Name);
        }

        public async Task<AuthResponseDto> LoginAsync(AuthLoginDto dto, UserInfo userInfo)
        {
            var user = await CheckUserExistAndGetByEmail(dto.Email);

            if (user == null)
                throw new BadRequestException("User not found");

            await ValidatePassword(user, dto.Password);

            BackgroundLoginWork(user.UserId, userInfo);

            return await IssueTokens(user, user.Role.Name);
        }

        public async Task<AuthResponseDto> Refresh(RefreshTokenDto request)
        {
            var storedToken = await CheckRefreshTokenIsValid(request.Token);
            var user = await CheckUserExistAndGetById(storedToken.UserId);

            if (user == null)
                throw new BadRequestException("User not found");

            storedToken.IsRevoked = true;

            await _refreshTokenRepo.UpdateAsync(storedToken);

            await _redisServcie.SetAsync("revoked", request.Token, "1", TimeSpan.FromDays(7));

            return await IssueTokens(user, user.Role.Name, storedToken.RefreshTokenId);
        }
    }
}
