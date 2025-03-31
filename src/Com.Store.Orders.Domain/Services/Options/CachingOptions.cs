namespace Com.Store.Orders.Domain.Services.Options
{
    public class CachingOptions
    {
        public const string SectionName = "Caching";

        public int ExpirationInMinutes { get; set; }

        public string Prefix { get; set; }
    }
}
