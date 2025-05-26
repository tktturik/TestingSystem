using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Authorization.EndPoints
{
    public class SendCode : BaseEndPointClass
    {
        int id;
        string code;
        string bearerToken;

        public SendCode(string _bearerToken, int _id, string _code) {
            id = _id;
            code = _code;
            bearerToken = _bearerToken;
        }
     
        public override async Task<string> GetDataAsync(HttpClient httpClient)
        {
            var requestData = new
            {
                id = this.id,
                code = this.code
            };

            AddAuthorizationHeader(httpClient, bearerToken);

            string _jsonResponse = await ExecutePostAsync(httpClient, "send_code", requestData);

            using (JsonDocument document = JsonDocument.Parse(_jsonResponse))
            {
                if (document.RootElement.TryGetProperty("status", out JsonElement status))
                {
                    return status.GetString();
                }
            }
            throw new Exception("Поле 'temporary_token' не найдено");
            
        }
    }
}
