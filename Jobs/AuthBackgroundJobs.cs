using WebApplication1.Common.Parsers;
using WebApplication1.Email.EmailTemplates;
using WebApplication1.Entities;
using WebApplication1.Interfaces;
using WebApplication1.Mappers;
using WebApplication1.Services;

namespace WebApplication1.Jobs
{
    public class AuthBackgroundJobs
    {
        private readonly IUserRepository _userRepo;
        private readonly IUserLoginHistoryRepository _loginHistoryRepo;
        private readonly EmailService _emailService;
        private readonly AuthMapper _mapper;
        private readonly Parser _parser;

        public AuthBackgroundJobs(IUserRepository userRepo, IUserLoginHistoryRepository loginHistoryRepo, EmailService emailService) 
        {
            _userRepo = userRepo;
            _loginHistoryRepo = loginHistoryRepo;
            _emailService = emailService;
            _mapper = new AuthMapper();
            _parser = new Parser();
        }

        private async Task<User?> GetUser(Guid userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            return user;
        }

        public async Task UserLoginHistroyAsync(Guid userId, UserInfo userInfo)
        {
            var loginHistoryEntity = _mapper.ToLoginHIstoryEntity(userId, userInfo);
            await _loginHistoryRepo.AddAsync(loginHistoryEntity);
        }

        public async Task SendLoginAlertEmailAsync(Guid userId, UserInfo userInfo)
        {
            var user = await GetUser(userId);
            if (user == null)
                return;

            var template = new LoginAlertEmailTemplate();
            var loginAlertTemplate = template.Template(user, userInfo);
            await _emailService.SendEmailAsync(loginAlertTemplate);
        }

        public async Task SendRegisterEmailAsync(Guid userId)
        {
            var user = await GetUser(userId);
            if (user == null)
                return;

            var template = new RegisterEmailTemplate();
            var registerTemplate = template.Template(user);
            await _emailService.SendEmailAsync(registerTemplate);
        }
    }
}
