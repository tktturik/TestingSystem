using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Authorization
{
    public abstract class BaseEndPointClass
    {
        /// <summary>
        /// Get Запрос на сервер
        /// </summary>
        /// <param name="_httpClient"></param>
        /// <param name="_endpoint">название EndPoint</param>
        /// <returns></returns>
        protected async Task<string> ExecuteGetAsync(HttpClient _httpClient, string _endpoint)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(_endpoint);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        /// <summary>
        /// Post запрос на сервер
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="endpoint">Название EndPoint</param>
        /// <param name="data">Тело запроса</param>
        /// <returns></returns>
        protected async Task<string> ExecutePostAsync(HttpClient httpClient,string endpoint,object data)
        {
            string jsonContent = JsonSerializer.Serialize(data);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
           
            HttpResponseMessage response = await httpClient.PostAsync(endpoint, content);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
        /// <summary>
        /// Добавление в заголовок Bearer токена
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="token">Bearer Token</param>
        protected void AddAuthorizationHeader(HttpClient httpClient, string token)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        /// <summary>
        /// Реализация получения данных с EndPoint
        /// </summary>
        /// <param name="httpClient"></param>
        /// <returns></returns>
        public abstract Task<string> GetDataAsync(HttpClient httpClient);
    }
}
