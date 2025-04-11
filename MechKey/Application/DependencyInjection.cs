using Application.Interfaces.IServices;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection service)
        {
            service.AddScoped<IJwtManager, JwtManager>();
            return service;
        }

    }
}
