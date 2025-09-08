using Application.Interfaces.IServices;
using Application.Services;
using Infrastructure.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Shared.Mapping.Implementations;
using Shared.Mapping.Interfaces;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection service)
        {
            //service.AddAutoMapper(typeof(AutoMapperProfile));

            // Register mapping services
            service.AddScoped<IApplicationUserMapping, ApplicationUserMapping>();
            service.AddScoped<IProductMapping, ProductMapping>();
            service.AddScoped<IProductRatingMapping, ProductRatingMapping>();
            service.AddScoped<IOrderItemMapping, OrderItemMapping>();
            service.AddScoped<IOrderMapping, OrderMapping>();
            service.AddScoped<ICategoryMapping, CategoryMapping>();

            // Register application services
            service.AddScoped<IApplicaionUserService, ApplicationUserService>();
            service.AddScoped<IAuthenticationService, AuthenticationService>();
            service.AddScoped<IProductService, ProductService>();
            service.AddScoped<IProductSalesTracker, ProductSalesTracker>();
            service.AddScoped<IProductRatingService, ProductRatingService>();
            service.AddScoped<ICategoryService, CategoryService>();
            service.AddScoped<IJwtManager, JwtManager>();
            service.AddScoped<IOrderService, OrderService>();
            service.AddScoped<IDashboardService, DashboardService>();

            return service;
        }
    }
}
