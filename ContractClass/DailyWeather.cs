using System.Collections.Generic;

namespace SmartEnergy.ContractClass
{
    public class DailyWeather
    {
        public string date { get; set; }
        public string day { get; set; }
        public IEnumerable<HourlyWeatherData> data { get; set; }
    }

    public class HourlyWeatherData
    {
        public string time { get; set; }
        public string temperature { get; set; }
        public string pressure { get; set; }
        public string windSpeed { get; set; }
        public string weatherCondition { get; set; }
    }

}

