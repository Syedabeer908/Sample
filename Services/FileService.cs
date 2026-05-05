using Microsoft.Extensions.Options;
using WebApplication1.Common.Exceptions;
using WebApplication1.Settings;

namespace WebApplication1.Services
{
    public class FileService
    {
        private readonly FileSettings _fileSettings;
        private readonly string _basePath;

        public FileService(IOptions<FileSettings> fileOptions)
        {
            _fileSettings = fileOptions.Value;
            _basePath = Path.Combine(_fileSettings.BasePath, "uploads");
        }

        private string GetContentType(string path)
        {
            var ext = Path.GetExtension(path).ToLower();

            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                _ => "application/octet-stream"
            };
        }

        private string CheckAndGetFullPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new BadRequestException("Invalid Path");

            var relativePath = path.Replace("/uploads/", "").TrimStart('/');

            var fullPath = Path.Combine(_basePath, relativePath);

            var normalizedBasePath = Path.GetFullPath(_basePath);

            var normalizedFullPath = Path.GetFullPath(fullPath);

            if (!normalizedFullPath.StartsWith(normalizedBasePath))
                throw new BadRequestException("Invalid Path");

            return normalizedFullPath;
        }

        public (Stream FileStream, string FileType)? GetFileStream(string path)
        {
            var filePath = CheckAndGetFullPath(path);

            if (!File.Exists(filePath))
                return null;

            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var fileType = GetContentType(filePath);

            return (stream, fileType);
        }

        public async Task<string> UploadFile(IFormFile file, string folderName)
        {
            var folderPath = Path.Combine(_basePath, folderName);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var extension = Path.GetExtension(file.FileName).ToLower();
            var fileName = Guid.NewGuid().ToString() + extension;
            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return $"/uploads/{folderName}/{fileName}".Replace("\\", "/");
        }

        public async Task<bool> DeleteFile(string path)
        {
            var filePath = CheckAndGetFullPath(path);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }

            return false;
        }
    }
}
