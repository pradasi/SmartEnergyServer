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

    public class WeeklyJson
    {
        public List<string> dayOfWeek { get; set; }
        public List<string> narrative { get; set; }
        public List<int?> temperatureMax { get; set; }
        public List<int?> temperatureMin { get; set; }
        public List<DateTime> validTimeLocal { get; set; }
    }
}
