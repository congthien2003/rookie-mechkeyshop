using Application.Interfaces.IApiClient.Redis;
using Application.Interfaces.IApiClient.Supabase;
using Application.Interfaces.IUnitOfWork;
using Domain.Entity;
using Domain.IRepositories;
using Infrastructure.ApiClient.Redis;
using Infrastructure.ApiClient.SupabaseCloud;
using Infrastructure.Repositories;
using Infrastructure.UnitOfWork;
using MechkeyShop.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {

            var connectionString = config.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped<IApplicationUserRepository<ApplicationUser>, ApplicationUserRepository>();
            services.AddScoped<IProductRepository<Product>, ProductRepository>();
            services.AddScoped<ICategoryRepository<Category>, CategoryRepository>();
            services.AddScoped<IProductRatingRepository<ProductRating>, ProductRatingRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderItemsRepository, OrderItemsRepository>();
            services.AddScoped<IOrderUnitOfWork, OrderUnitOfWork>();
            services.AddScoped<IProductUnitOfWork, ProductUnitOfWork>();
            services.AddScoped<ISupabaseService, SupabaseService>();

            services.AddScoped<IRedisService, RedisService>();

            return services;
        }
    }
}
