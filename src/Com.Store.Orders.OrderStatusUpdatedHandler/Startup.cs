using Com.Store.Orders.Domain.Data.Extensions;
using Com.Store.Orders.Domain.Data.Options;
using Com.Store.Orders.Domain.Services.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Com.Store.Orders.OrderStatusUpdatedHandler;

[Amazon.Lambda.Annotations.LambdaStartup]
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        var builder = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json", true);
        builder.AddSystemsManager("/app/settings");
        var configuration = builder.Build();
        services.AddSingleton<IConfiguration>(configuration);
        services.Configure<ConnectionStringsOptions>(options =>
        {
            configuration.GetSection(ConnectionStringsOptions.SectionName).Bind(options);
        });
        services.AddDataLayer(configuration);
        services.AddDomainLayer(configuration);
    }
}
