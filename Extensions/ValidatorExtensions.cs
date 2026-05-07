using FluentValidation;
using FluentValidation.AspNetCore;
using WebApplication1.Validator;

namespace WebApplication1.Extensions
{
    public static class ValidatorExtensions
    {
        public static IServiceCollection AddValidatorServices(this IServiceCollection services, IConfiguration config)
        {
            services
                .AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters();

            services.AddValidatorsFromAssemblyContaining<UploadProfileImageRequestValidator>();

            return services;
        }
    }
}
