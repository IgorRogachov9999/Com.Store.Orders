using Amazon.SQS;
using Com.Store.Orders.Domain.Services.Mapper;
using Com.Store.Orders.Domain.Services.Services;
using Com.Store.Orders.Domain.Services.Services.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Com.Store.Orders.Domain.Services.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDomainLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IEventProducingService, SqsEventProducingService>();
            services.AddScoped<IUserService, UserService>();
            services.AddAWSService<IAmazonSQS>();
            services.AddAutoMapper(typeof(MappingProfile));

            return services;
        }
    }
}
