using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testingSystem.Models
{
    /// <summary>
    /// Сущность Попытки, включает 
    /// LastUpdate - Дата последнего обновления попыток
    /// CountAttemps - Количество оставшихся попыток
    /// </summary>
    public class Attemps
    {
        public DateTime LastUpdate { get; set; }
        public int CountAttemps {  get; set; }

        /// <summary>
        /// Обновление попыток и сериализация текущего объекта
        /// </summary>
        /// <returns>Количество попыток</returns>
        public int UpdateAttemps()
        {
            CountAttemps = 1;
            LastUpdate = DateTime.Now;
            DataService.SerializeAttemps(this);
            return CountAttemps;
        }
     
        
        public static Attemps operator --(Attemps attemps)
        {
            attemps.CountAttemps--;
            DataService.SerializeAttemps(attemps);
            return attemps;
        }
    }
}
