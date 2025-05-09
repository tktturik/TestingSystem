using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace Authorization
{
    public class Authorization
    {
        string phone;
        string temp_token;
        private readonly HttpClient httpClient;
        string baseUrl = "https://damfold.duckdns.org/api/";

        string Phone { get; set; }
        string TempToken { get; set; }
        public Authorization(string _phone)
        {
            Phone = _phone;
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("accept", "application/json");
            httpClient.BaseAddress = new Uri(baseUrl);
        }
        public async Task<string> GetTemporaryTokenAsync()
        {
            try
            {
                const string _endpoint = "temporary_token";
                HttpResponseMessage _response = await httpClient.GetAsync(_endpoint);

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
                throw new Exception("Field 'temporary_token' not found in response");
            }
            catch (HttpRequestException httpEx)
            {
                throw new Exception("HTTP request failed", httpEx);
            }
            catch (JsonException jsonEx)
            {
                throw new Exception("Failed to parse response", jsonEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while getting temporary token", ex);
            }
        }

        public async Task<string> ConnectionAndGetDataEP(IEndPointStrategy endPoint)
        {
            try
            {
                return await endPoint.GetDataAsync();
            }
            catch (HttpRequestException httpEx)
            {
                throw new Exception("HTTP request failed", httpEx);
            }
            catch (JsonException jsonEx)
            {
                throw new Exception("Failed to parse response", jsonEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while getting temporary token", ex);
            }
        }
        public async Task<string> GetUsers(string _tempToken)
        {

        }

    }
}
