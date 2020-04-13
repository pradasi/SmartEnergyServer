using System;
using System.Collections.Generic;

namespace SmartEnergy.ContractClass
{
    public class DailyWeather
    {
        public string date { get; set; }
        public string day { get; set; }
        public List<HourlyWeatherData> data { get; set; }
    }

    public class HourlyWeatherData
    {
        public string time { get; set; }
        public string temperature { get; set; }
        public string pressure { get; set; }
        public string windSpeed { get; set; }
        public string weatherCondition { get; set; }
    }

    public class HourlyWeather
    {
        public List<string> dayOfWeek { get; set; }
        public List<DateTime> validTimeLocal { get; set; }
        public List<int> temperature { get; set; }
        public List<float> pressureMeanSeaLevel { get; set; }
        public List<int> windSpeed { get; set; }
        public List<string> wxPhraseLong { get; set; }
    }

}

