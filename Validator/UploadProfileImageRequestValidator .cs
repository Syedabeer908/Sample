using FluentValidation;
using WebApplication1.DTOs;

namespace WebApplication1.Validator
{
    public class UploadProfileImageRequestValidator : AbstractValidator<CreateProfileImageDto>
    {
        public UploadProfileImageRequestValidator()
        {
            RuleFor(x => x.File)
                .NotNull()
                .WithMessage("File is required");

            RuleFor(x => x.File.Length)
                .GreaterThan(0)
                .WithMessage("File cannot be empty")
                .LessThanOrEqualTo(2 * 1024 * 1024)
                .WithMessage("Max file size is 2MB");

            RuleFor(x => x.File.FileName)
                .Must(BeValidExtension)
                .WithMessage("Only .jpg, .jpeg, .png are allowed");
        }

        private bool BeValidExtension(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return false;

            var allowed = new[] { ".jpg", ".jpeg", ".png" };
            var ext = Path.GetExtension(fileName).ToLower();

            return allowed.Contains(ext);
        }
    }
}
