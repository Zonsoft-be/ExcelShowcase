using Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProductManager.Services
{
    public class HttpService : IHttpService
    {
        public async Task<T> Load<T>(string baseAddress, string requestUri)
        {
            var httpClient = this.CreateClient(baseAddress);

            HttpResponseMessage response = await httpClient.GetAsync(requestUri);
            if (response.IsSuccessStatusCode)
            {
                var rawData = await response.Content.ReadAsStreamAsync();
                var valueTask = await System.Text.Json.JsonSerializer.DeserializeAsync<T>(rawData);

                return valueTask;
            }

            return default(T);
        }

        public HttpClient CreateClient(string baseAddress)
        {
            var httpClientHandler = new HttpClientHandler { UseDefaultCredentials = true };
            var httpClient = new HttpClient(httpClientHandler)
            {
                BaseAddress = new Uri(baseAddress),
                Timeout = Timeout.InfiniteTimeSpan
            };

            return httpClient;
        }
    }
}
