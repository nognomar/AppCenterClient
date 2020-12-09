using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AppCenterClient.Utils
{
    public static class HttpExtensions
    {
        public static async Task<TResponse> AppCenterPostRequest<TResponse>(this HttpClient client, string requestUri, object? content = null) where TResponse : class
        {
            var response = await client.AppCenterSendJsonRequest(HttpMethod.Post, requestUri, content);
            return await response.DeserializeContent<TResponse>();
        }

        public static async Task<TResponse> AppCenterPatchRequest<TResponse>(this HttpClient client, string requestUri, object? content = null) where TResponse : class
        {
            var response = await client.AppCenterSendJsonRequest(HttpMethod.Patch, requestUri, content);
            return await response.DeserializeContent<TResponse>();
        }

        public static async Task<TResponse> AppCenterGetRequest<TResponse>(this HttpClient client, string requestUri) where TResponse : class
        {
            var response = await client.AppCenterSendJsonRequest(HttpMethod.Get, requestUri);
            return await response.DeserializeContent<TResponse>();
        }

        public static async Task<TResponse> AppCenterUploadRequest<TResponse>(this HttpClient client, string requestUri, byte[] content) where TResponse : class
        {
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri) {Content = new ByteArrayContent(content)};
            request.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/octet-stream");
            request.Content.Headers.ContentLength = content.Length;

            Console.WriteLine($"Create API Upload Request: AppCenter='{client.BaseAddress}', Request='{requestUri}', Method='{HttpMethod.Post}', RequestContentLength='{content.Length}'");

            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.DeserializeContent<TResponse>();
            }

            var responseContent = await response.DeserializeContent<string>();
            throw new HttpRequestException(
                $"API Upload Request failed: ResponseStatusCode='{response.StatusCode}', ResponseContent='{responseContent}'"
            );
        }

        private static async Task<HttpResponseMessage> AppCenterSendJsonRequest(this HttpClient client, HttpMethod method, string requestUri, object? content = null)
        {
            var request = new HttpRequestMessage(method, requestUri);
            var requestContent = string.Empty;
            if (content != null)
            {
                requestContent = JsonSerializer.Serialize(content);
                request.Content = new StringContent(requestContent, Encoding.UTF8, "application/json");
            }

            Console.WriteLine($"Create API Request: AppCenter='{client.BaseAddress}', Request='{requestUri}', Method='{method}', RequestContent='{requestContent}'");

            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return response;
            }

            var responseContent = await response.DeserializeContent<string>();
            throw new HttpRequestException(
                $"API Request failed: ResponseStatusCode='{response.StatusCode}', ResponseContent='{responseContent}'"
            );
        }

        private static async Task<TResponse> DeserializeContent<TResponse>(this HttpResponseMessage response) where TResponse : class
        {
            var content = await response.Content.ReadAsStringAsync();
            if (typeof(TResponse) == typeof(string) && content is TResponse stringContent)
            {
                return stringContent;
            }

            var obj = JsonSerializer.Deserialize<TResponse>(content);
            if (obj != null)
            {
                return obj;
            }

            throw new SerializationException($"Can't deserialize Content='{content}' to Type='{typeof(TResponse)}'");
        }
    }
}