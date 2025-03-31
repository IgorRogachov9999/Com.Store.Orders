using AutoFixture;
using AutoFixture.AutoMoq;
using Com.Store.Orders.Api.Tests.Infrastructure.Customizations;
using Com.Store.Orders.Api.Tests.Infrastructure.Http;
using Com.Store.Orders.Domain.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Com.Store.Orders.Api.Tests.Infrastructure
{
    public abstract class TestSuteBase : IClassFixture<OrdersWebApplicationFactory>
    {
        protected readonly IServiceScope _scope;
        protected readonly OrdersDbContext _dbContext;
        protected readonly ApiClient _client;
        protected readonly IFixture _fixture;

        public TestSuteBase(OrdersWebApplicationFactory factory)
        {
            _scope = factory.Services.CreateScope();
            _dbContext = _scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
            _client = new ApiClient(factory.CreateClient());
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new AutoMapperCustomization());
        }
    }
}
