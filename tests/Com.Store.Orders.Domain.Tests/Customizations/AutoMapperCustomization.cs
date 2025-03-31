using AutoFixture;
using AutoMapper;

namespace Com.Store.Orders.Domain.Tests.Customizations
{
    public class AutoMapperCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(AppDomain.CurrentDomain.GetAssemblies());
            });

            var mapper = config.CreateMapper();
            fixture.Inject(mapper);
        }
    }
}
