namespace Com.Store.Orders.Domain.Services.Exceptions
{
    public class DomainEntityNotFoundException : DomainException
    {
        public DomainEntityNotFoundException(string errorMessage) : base(errorMessage, null)
        {
        }

        public override int ErrorCode => 404;
    }
}
