using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Authorization.EndPoints
{
    public class TemporaryToken : IEndPointStrategy
    {
        public async Task<string> GetDataAsync()
        {
            const string _endpoint = "temporary_token";
            HttpResponseMessage _response = await HttpClient.GetAsync(_endpoint);

            _response.EnsureSuccessStatusCode();

            string _jsonResponse = await _response.Content.ReadAsStringAsync();


            using (JsonDocument document = JsonDocument.Parse(_jsonResponse))
            {
                if (document.RootElement.TryGetProperty("temporary_token", out JsonElement tokenElement))
                {
                    TempToken = tokenElement.GetString();
                    return TempToken;
                }
            }
            throw new Exception("Field 'temporary_token' not found in response")        }
    }
}
