using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartEnergy.ContractClass
{
    public class WeeklyWeather
    {
        public string date { get; set; }
        public string day { get; set; }
        public string maxTemperature { get; set; }
        public string minTemperature { get; set; }
        public string weatherCondition { get; set; }
    }
}
