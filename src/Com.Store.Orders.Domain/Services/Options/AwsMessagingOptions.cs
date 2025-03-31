namespace Com.Store.Orders.Domain.Services.Options
{
    public class AwsMessagingOptions
    {
        public const string SectionName = "AwsMessaging";

        public string Source { get; set; }

        public Dictionary<string, string> Queues { get; set; } = new();
    }
}
