namespace Com.Store.Orders.Domain.Services.Exceptions
{
    public class MissingQueueConfigurationException : DomainException
    {
        public MissingQueueConfigurationException(string errorMessage) : base(errorMessage, null)
        {
        }

        public override int ErrorCode => 500;
    }
}
