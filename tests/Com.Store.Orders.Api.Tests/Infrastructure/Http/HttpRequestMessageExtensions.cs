using System.Text;
using System.Text.Json;

namespace Com.Store.Orders.Api.Tests.Infrastructure.Http
{
    public static class HttpRequestMessageExtensions
    {
        public static void AddJsonContent(this HttpRequestMessage request, object? body)
        {
            request.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
        }

        public static void AddMultipartFormDataContent(this HttpRequestMessage request, MultipartFormDataContent? formData)
        {
            request.Content = formData;
        }
    }
}
