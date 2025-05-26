using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using static System.Net.WebRequestMethods;
using Authorization.EndPoints;
using System.Globalization;

namespace Authorization
{
    public class Auth
    {
        string temp_token;
        private readonly HttpClient httpClient;

        /// <summary>
        /// URL сервера с EndPoint
        /// </summary>
        string baseUrl = "https://damfold.duckdns.org/api/";

        /// <summary>
        /// Временный токен для доступа к EndPoint
        /// </summary>
        public string TempToken {
            get {
                return temp_token;
            }
            set
            {
                temp_token = value;
            }
        }
        public Auth()
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("accept", "application/json");
            httpClient.BaseAddress = new Uri(baseUrl);
        }
      
        /// <summary>
        /// Вызов и получение данных EndPoint на сервере
        /// </summary>
        /// <param name="endPoint">Класс конкретного EndPoint к которому идет обращение </param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> CallEndpointAsync(BaseEndPointClass endPoint)
        {
            try
            {
                return await endPoint.GetDataAsync(httpClient);
            }
            catch (HttpRequestException httpEx)
            {
                throw new Exception("Ошибка при выполнении HTTP-запроса", httpEx);
            }
            catch (JsonException jsonEx)
            {
                throw new Exception("Не удалось распарсить ответ", jsonEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка", ex);
            }
        }
        /// <summary>
        /// Получение списка пользователей с сервера
        /// </summary>
        /// <returns>Словарь строк, где ключ - id, а значение это ФИО пользователя</returns>
        public async Task<Dictionary<int,string>> GetUsers()
        {
            TempToken = await CallEndpointAsync(new TemporaryToken());
            string _json_of_users = await CallEndpointAsync(new GetUsers(TempToken));

            using (JsonDocument document = JsonDocument.Parse(_json_of_users))
            {
                return document.RootElement.EnumerateArray()
            .ToDictionary(
                user => user.GetProperty("id").GetInt32(),
                user =>
                {
                    string lastName = user.GetProperty("last_name").GetString() ?? "";
                    string firstName = user.GetProperty("first_name").GetString() ?? "";
                    string middleName = user.GetProperty("middle_name").GetString() ?? "";

                    return $"{lastName} {firstName} {middleName}".Trim();
                }
            );
            }
        }
        /// <summary>
        /// Генерация и отправка кода пользователю в ТГ
        /// </summary>
        /// <param name="_phone">Номер пользователя</param>
        /// <returns></returns>
        public async Task<(string,string)> SendCodeTG(int _id)
        {
            string _code = Code.GenerateCode(_id);
            string _jsonResponse = await CallEndpointAsync(new SendCode(TempToken, _id, _code));
            return (_code, _jsonResponse);
        }
        /// <summary>
        /// Верификация пользователя и получение основного токена
        /// </summary>
        /// <param name="_phone">Номер пользователя</param>
        /// <returns>Основной токен и данные текущего пользователя</returns>
        public async Task<(string token, Dictionary<string,object> user)> Verify(int _id)
        {

            string _response = await CallEndpointAsync(new Verify(TempToken, _id));

            using (JsonDocument _document = JsonDocument.Parse(_response))
            {
                JsonElement root = _document.RootElement;
                string token = root.GetProperty("access_token").GetString();

                Dictionary<string,object> _userDict = new Dictionary<string, object>
                {
                    ["first_name"] = root.GetProperty("user").GetProperty("first_name").GetString(),
                    ["last_name"] = root.GetProperty("user").GetProperty("last_name").GetString(),
                    ["phone_number"] = root.GetProperty("user").GetProperty("phone_number").GetString(),
                    ["chat_id"] = root.GetProperty("user").GetProperty("chat_id").GetInt64(),
                    ["role"] = root.GetProperty("user").GetProperty("role").GetString(),
                    ["points"] = root.GetProperty("user").GetProperty("points").GetInt32()
                };

                return (token, _userDict);
            }
        }
       
      

    }
}
