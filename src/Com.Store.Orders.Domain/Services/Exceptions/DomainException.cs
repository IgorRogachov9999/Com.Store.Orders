namespace Com.Store.Orders.Domain.Services.Exceptions
{
    public abstract class DomainException : Exception
    {
        protected DomainException(Exception? innerException) : base(null, innerException) { }

        protected DomainException(string errorMessage, Exception? innerException) : this(innerException)
        {
            ErrorMessage = errorMessage;
        }

        public virtual string? ErrorMessage { get; }

        public abstract int ErrorCode { get; }
    }
}
