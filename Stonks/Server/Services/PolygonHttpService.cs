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
    public class PolygonStockDetails
    {
        public Uri Logo { get; set; }

        public DateTimeOffset Listdate { get; set; }

        public string Cik { get; set; }

        public string Bloomberg { get; set; }
        
        public object Figi { get; set; }

        public string Lei { get; set; }
        
        public long Sic { get; set; }
        
        public string Country { get; set; }

        public string Industry { get; set; }

        public string Sector { get; set; }

        public long Marketcap { get; set; }

        public long Employees { get; set; }

        public string Phone { get; set; }

        public string Ceo { get; set; }

        public Uri Url { get; set; }

        public string Description { get; set; }

        public string Exchange { get; set; }

        public string Name { get; set; }

        public string Symbol { get; set; }

        public string ExchangeSymbol { get; set; }

        public string HqAddress { get; set; }

        public string HqState { get; set; }
        
        public string HqCountry { get; set; }
        
        public string Type { get; set; }
        
        public string Updated { get; set; }
        
        public string[] Tags { get; set; }

        public string[] Similar { get; set; }
        
        public bool Active { get; set; }
    }
    
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

        public async Task<HttpResponseWrapper<PolygonStockDetails>> FindStockByTicker(string ticker)
        {
            return await Get<PolygonStockDetails>($"/v1/meta/symbols/{ticker}/company");
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
