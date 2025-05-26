using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Authorization.EndPoints
{
    public class TemporaryToken: BaseEndPointClass
    {  
        /// <summary>
        /// Получение временного токена
        /// </summary>
        /// <param name="httpClient"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public override async Task<string> GetDataAsync(HttpClient httpClient)
        {

            string _jsonResponse = await ExecuteGetAsync(httpClient, "temporary_token");

            using (JsonDocument document = JsonDocument.Parse(_jsonResponse))
            {
                if (document.RootElement.TryGetProperty("temporary_token", out JsonElement tokenElement))
                {
                    return tokenElement.GetString();
                }
            }
            throw new Exception("Поле 'temporary_token' не найдено");
        }
    }
}
