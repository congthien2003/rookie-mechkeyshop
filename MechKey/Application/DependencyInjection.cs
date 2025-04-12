using Application.Interfaces.IServices;
using Application.Services;
using Infrastructure.Helpers;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection service)
        {
            service.AddAutoMapper(typeof(AutoMapperProfile));
            service.AddScoped<IApplicaionUserService, ApplicationUserService>();
            service.AddScoped<IAuthenticationService, AuthenticationService>();

            service.AddScoped<IJwtManager, JwtManager>();
            return service;
        }

        public static IServiceCollection AddServicesFromAssembly(this IServiceCollection services, Assembly assembly)
        {
            var types = assembly.GetTypes();

            var interfaces = types.Where(t => t.IsInterface && t.Name.StartsWith("I")).ToList();
            var implementations = types.Where(t => t.IsClass && !t.IsAbstract).ToList();

            foreach (var @interface in interfaces)
            {
                var implementation = implementations
                    .FirstOrDefault(x => @interface.IsAssignableFrom(x));

                if (implementation != null)
                {
                    services.AddTransient(@interface, implementation);
                }
            }

            return services;
        }

    }
}
