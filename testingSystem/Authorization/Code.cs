using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Authorization
{
    public static class Code
    {
        /// <summary>
        /// Генерация кода на основе случайного числа, номера телефона и времени
        /// </summary>
        /// <param name="id">Id пользоватля</param>
        /// <returns>6-ти значный код</returns>
        public static string GenerateCode(int _id)
        {
            Random _rnd = new Random();
            int _randomPart = _rnd.Next(100000, 999999);
            int _timePart = DateTime.Now.Second * 10000 + DateTime.Now.Millisecond;
            int _phonePart = _id.GetHashCode();

            int _code = Math.Abs(_randomPart ^ _timePart ^ _phonePart) % 900000 + 100000;
            return _code.ToString();
        }
    }
}
