using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Authorization.EndPoints
{
    public class GetUsers : BaseEndPointClass
    {
        /// <summary>
        /// Временный токен Bearer
        /// </summary>
        private string bearerToken;

        public GetUsers(string _bearerToken)
        {
            bearerToken = _bearerToken;
        }
        /// <summary>
        /// Получение json строки со списком пользователей
        /// </summary>
        /// <param name="httpClient"></param>
        /// <returns>json строка со списком пользователей</returns>
        public override async Task<string> GetDataAsync(HttpClient httpClient)
        {
            AddAuthorizationHeader(httpClient,bearerToken);
            string _jsonResponse = await ExecuteGetAsync(httpClient, "get_users");

            return _jsonResponse;
        }
    }
}
