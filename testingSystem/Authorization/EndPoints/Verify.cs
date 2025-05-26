using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Authorization.EndPoints
{
    public class Verify : BaseEndPointClass
    {
        string bearerToken;
        int id;
        string flag = "SUCCESS_ACCESS";
        public Verify(string _bearerToken, int _id) 
        {
            bearerToken = _bearerToken;
            id = _id;
        }
        /// <summary>
        /// Верификация пользователя
        /// </summary>
        /// <param name="httpClient"></param>
        /// <returns>json строка с токеном и данными авторизированного пользователя</returns>
        public override async Task<string> GetDataAsync(HttpClient httpClient)
        {
            var requestData = new
            {
                flag = this.flag,
                id = this.id
            };

            AddAuthorizationHeader(httpClient, bearerToken);

            return await ExecutePostAsync(httpClient, "verify", requestData);
        }
    }
}
