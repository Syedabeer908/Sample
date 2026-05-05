using WebApplication1.Entities;
using WebApplication1.Interfaces;
using WebApplication1.Common.Exceptions;

namespace WebApplication1.Services
{
    public class ProfileService
    {
        private readonly FileService _fileService;
        private readonly IUserRepository _repo;

        public ProfileService(FileService fileService, IUserRepository repo)
        {
            _fileService = fileService;
            _repo = repo;
        }

        private string BuildFolderPath(Guid id)
        {
            return "users/" + id + "/profile";
        }

        public async Task<User> CheckUserExistAndGet(Guid targetUserId)
        {
            var user = await _repo.GetByIdAsync(targetUserId);

            if (user == null)
                throw new NotFoundException($"User with id {targetUserId} not found.");
            return user;
        }

        public (Stream FileStream, string FileType)? GetProfileStream(User user)
        {
            if (user.ProfileImagePath == null)
                throw new NotFoundException("File not found.");

            return _fileService.GetFileStream(user.ProfileImagePath);
        }

        public async Task UpdateProfileImage(Guid userId, IFormFile file)
        {
            var user = await CheckUserExistAndGet(userId);

            var folderPath = BuildFolderPath(userId);
            var imagePath = await _fileService.UploadFile(file, folderPath);

            await DeleteProfile(user, imagePath);
        }

        public async Task DeleteProfile(User user, string? filepath = null )
        {
            if (!string.IsNullOrEmpty(user.ProfileImagePath))
            {
                await _fileService.DeleteFile(user.ProfileImagePath);
            }

            user.ProfileImagePath = filepath;
            await _repo.UpdateAsync(user);
        }
    }
}
