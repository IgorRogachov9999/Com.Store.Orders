using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using Com.Store.Orders.Domain.Data;
using Com.Store.Orders.Domain.Services.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.LocalStack;
using Testcontainers.PostgreSql;

namespace Com.Store.Orders.Api.Tests.Infrastructure
{
    public class OrdersWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithDatabase("orders")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        private readonly LocalStackContainer _sqsContainer = new LocalStackBuilder()
            .WithImage("localstack/localstack:4.2.0")
            .WithCleanUp(true)
            .WithEnvironment("DEFAULT_REGION", "eu-central-1")
            .WithEnvironment("AWS_ACCESS_KEY_ID", "123")
            .WithEnvironment("AWS_SECRET_ACCESS_KEY", "123")
            .WithEnvironment("SERVICES", "sqs")
            .WithEnvironment("DOCKER_HOST", "unix:///var/run/docker.sock")
            .WithEnvironment("DEBUG", "1")
            .WithPortBinding(4566, 4566)
            .Build();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                AddDbContext(services);
                AddSqsClient(services);
            });

            builder.UseEnvironment("Development");
        }

        private void AddSqsClient(IServiceCollection services)
        {
            var sqsDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IAmazonSQS));
            services.Remove(sqsDescriptor);

            var sqsConfig = new AmazonSQSConfig()
            {
                RegionEndpoint = RegionEndpoint.EUCentral1,
                UseHttp = true,
                ServiceURL = "http://localhost:4566/"
            };
            var sqsClient = new AmazonSQSClient("123", "123", sqsConfig);

            services.AddSingleton<IAmazonSQS>(sqsClient);
            var queueUrl = CreateQueue(sqsClient);
            services.Configure<AwsMessagingOptions>(options =>
            {
                options.Queues = new Dictionary<string, string>
                {
                    { "OrderStatusUpdated", queueUrl }
                };
            });
        }

        private string CreateQueue(AmazonSQSClient client)
        {
            var createQueueRequest = new CreateQueueRequest()
            {
                QueueName = "order-status-updated"
            };
            var response = client.CreateQueueAsync(createQueueRequest).GetAwaiter().GetResult();
            return response.QueueUrl;
        }

        private void AddDbContext(IServiceCollection services)
        {
            var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IDbContextOptionsConfiguration<OrdersDbContext>));
            services.Remove(dbContextDescriptor);

            services.AddDbContext<OrdersDbContext>((container, options) =>
            {
                options.UseNpgsql(_dbContainer.GetConnectionString());
            });
        }

        public async Task InitializeAsync()
        {
            await Task.WhenAll(
                _dbContainer.StartAsync(),
                _sqsContainer.StartAsync());
        }

        async Task IAsyncLifetime.DisposeAsync()
        {
            await Task.WhenAll(
                _dbContainer.StopAsync(),
                _sqsContainer.StopAsync());
        }
    }
}
