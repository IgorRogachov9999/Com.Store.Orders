namespace Com.Store.Orders.Domain.Data.Options
{
    public class ConnectionStringsOptions
    {
        public const string SectionName = "ConnectionStrings";

        public string OrdersDbConnectionString { get; set; }

        public string RedisConnectionString { get; set; }
    }
}
