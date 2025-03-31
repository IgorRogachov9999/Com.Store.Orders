using System.Net;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Com.Store.Orders.Api.Tests.Infrastructure.Http
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;

        public ApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<ApiResponse<TResponse>> GetAsync<TResponse>(string url)
            => MakeRequest<TResponse>(HttpMethod.Get, url, null);

        public Task<ApiResponse<TResponse>> PostAsync<TResponse>(string url, object body)
            => MakeRequest<TResponse>(HttpMethod.Post, url, body);

        public Task<ApiResponse<TResponse>> PutAsync<TResponse>(string url, object body)
            => MakeRequest<TResponse>(HttpMethod.Put, url, body);

        public Task<ApiResponse<TResponse>> DeleteAsync<TResponse>(string url)
            => MakeRequest<TResponse>(HttpMethod.Delete, url, null);

        private async Task<ApiResponse<TResponse>> MakeRequest<TResponse>(HttpMethod method, string url, object? body)
        {
            var request = new HttpRequestMessage(method, new Uri(url, UriKind.Relative));
            request.AddJsonContent(body);

            var response = await _httpClient.SendAsync(request);

            return await ReadAsync<TResponse>(response);
        }

        private static async Task<ApiResponse<T>> ReadAsync<T>(HttpResponseMessage response)
            => new ApiResponse<T>
            {
                StatusCode = (int)response.StatusCode,
                ReasonPhrase = response.ReasonPhrase,
                Body = await ReadBodyAsync<T>(response),
                ErrorMessage = response.IsSuccessStatusCode
                ? null
                : await response.Content.ReadAsStringAsync(),
                RequestUri = response.RequestMessage?.RequestUri?.OriginalString,
                RequestMethod = response.RequestMessage?.Method.ToString()
            };

        private static async Task<T?> ReadBodyAsync<T>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NoContent)
            {
                var responseContetnt = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                options.Converters.Add(new JsonStringEnumConverter());
                return JsonSerializer.Deserialize<T>(responseContetnt, options);
            }

            return default;
        }
    }
}
