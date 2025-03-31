namespace Com.Store.Orders.Api.Tests.Infrastructure.Http
{
    public record class ApiResponse<T> : ApiResponse
    {
        public T? Body { get; init; }
    }

    public record class ApiResponse
    {
        public int StatusCode { get; init; }

        public string? ReasonPhrase { get; init; }

        public string? ErrorMessage { get; init; }

        public string? RequestUri { get; init; }

        public string? RequestMethod { get; init; }

        public void EnsureStatusCode(int statusCode)
        {
            if (StatusCode == statusCode)
            {
                return;
            };

            throw new HttpRequestException(
                $@"The api response does not have the expected {statusCode} status code.
                   Request URI: {RequestUri}
                   Request method: {RequestMethod}
                   Response status code: {StatusCode} {ReasonPhrase}
                   Error message: {ErrorMessage}");
        }
    }
}
