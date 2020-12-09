using System;
using System.Net.Http;
using System.Threading.Tasks;
using AppCenterClient.Utils;

namespace AppCenterClient.Commands
{
    public abstract class AppCenterService : IDisposable
    {
        private readonly HttpClient _httpClient = new HttpClient();

        protected AppCenterService(string baseUrl, string token)
        {
            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.Timeout = TimeSpan.FromSeconds(600);
            _httpClient.DefaultRequestHeaders.Add("X-API-Token", token);
        }

        protected Task<TResponse> Post<TResponse>(string requestUri, object? content = null) where TResponse : class => _httpClient.AppCenterPostRequest<TResponse>(requestUri, content);

        protected Task<TResponse> Patch<TResponse>(string requestUri, object? content = null) where TResponse : class => _httpClient.AppCenterPatchRequest<TResponse>(requestUri, content);

        protected Task<TResponse> Get<TResponse>(string requestUri) where TResponse : class => _httpClient.AppCenterGetRequest<TResponse>(requestUri);

        public void Dispose()
        {
            _httpClient.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}