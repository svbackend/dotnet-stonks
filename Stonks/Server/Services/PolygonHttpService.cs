using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Serialization;
using Stonks.Server.Utils;

namespace Stonks.Server.Services
{
    public class PolygonHttpService
    {
        private const string BaseUrl = "https://api.polygon.io";
        
        private readonly HttpClient _httpClient;

        private JsonSerializerOptions defaultJsonSerializerOptions =>
            new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance
            };

        public PolygonHttpService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(BaseUrl);

            var polygonApiKey = configuration.GetValue<string>("PolygonApiKey");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", polygonApiKey);
        }

        public async Task<HttpResponseWrapper<PolygonTickersResponse>> FindStocks(string query)
        {
            return await Get<PolygonTickersResponse>($"/v3/reference/tickers?search={query}&active=true&limit=10");
        }

        private async Task<HttpResponseWrapper<T>> Get<T>(string url)
        {
            var httpResponseMessage = await _httpClient.GetAsync(url);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var response = await Deserialize<T>(httpResponseMessage, defaultJsonSerializerOptions);
                return new HttpResponseWrapper<T>(response, true, httpResponseMessage);
            }
            else
            {
                return new HttpResponseWrapper<T>(default, false, httpResponseMessage);
            }
        }

        private async Task<HttpResponseWrapper<object>> Post<T>(string url, T data)
        {
            var dataJson = JsonSerializer.Serialize(data);
            var stringContent = new StringContent(dataJson, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, stringContent);
            return new HttpResponseWrapper<object>(null, response.IsSuccessStatusCode, response);
        }

        private async Task<HttpResponseWrapper<object>> Put<T>(string url, T data)
        {
            var dataJson = JsonSerializer.Serialize(data);
            var stringContent = new StringContent(dataJson, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(url, stringContent);
            return new HttpResponseWrapper<object>(null, response.IsSuccessStatusCode, response);
        }

        private async Task<HttpResponseWrapper<TResponse>> Post<T, TResponse>(string url, T data)
        {
            var dataJson = JsonSerializer.Serialize(data);
            var stringContent = new StringContent(dataJson, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, stringContent);
            if (response.IsSuccessStatusCode)
            {
                var responseDeserialized = await Deserialize<TResponse>(response, defaultJsonSerializerOptions);
                return new HttpResponseWrapper<TResponse>(responseDeserialized, true, response);
            }
            else
            {
                return new HttpResponseWrapper<TResponse>(default, false, response);
            }
        }

        private async Task<HttpResponseWrapper<object>> Delete(string url)
        {
            var httpResponseMessage = await _httpClient.DeleteAsync(url);
            return new HttpResponseWrapper<object>(null, httpResponseMessage.IsSuccessStatusCode, httpResponseMessage);
        }

        private async Task<T> Deserialize<T>(HttpResponseMessage httpResponse, JsonSerializerOptions options)
        {
            var responseString = await httpResponse.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(responseString, options);
        }
    }
    
}
