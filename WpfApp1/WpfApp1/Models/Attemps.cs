using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testingSystem.Models
{
    public class Attemps
    {
        public DateTime LastUpdate { get; set; }
        public int CountAttemps {  get; set; }

        public Attemps() {
        }

        public int UpdateAttemps()
        {
            CountAttemps = 3;
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
