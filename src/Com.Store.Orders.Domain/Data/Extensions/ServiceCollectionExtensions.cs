using Com.Store.Orders.Domain.Data.Options;
using Com.Store.Orders.Domain.Data.Repositories;
using Com.Store.Orders.Domain.Data.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Com.Store.Orders.Domain.Data.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDataLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IItemRepository, ItemRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            
            services.AddDbContext<OrdersDbContext>((serviceProvider, options) =>
            {
                var connectionString = serviceProvider.GetRequiredService<IOptions<ConnectionStringsOptions>>().Value;
                options.UseNpgsql(connectionString.OrdersDbConnectionString);
            });

            return services;
        }
    }
}
