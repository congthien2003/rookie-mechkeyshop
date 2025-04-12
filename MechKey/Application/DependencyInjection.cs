using Application.Interfaces.IServices;
using Application.Services;
using Infrastructure.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection service)
        {
            service.AddAutoMapper(typeof(AutoMapperProfile));
            service.AddTransient<IApplicaionUserService, ApplicationUserService>();

            service.AddScoped<IJwtManager, JwtManager>();
            return service;
        }

    }
}
