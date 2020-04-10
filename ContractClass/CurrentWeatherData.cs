using System;

namespace SmartEnergy.ContractClass
{
    public class CurrentWeatherData
    {
        public string cloudPhrase { get; set; }
        public string dayOfWeek { get; set; }
        public string dayOrNight { get; set; }
        public string visibility { get; set; }
        public string pressure { get; set; }
        public string relativeHumidity { get; set; }
        public string temperature { get; set; }
        public string dewPoint { get; set; }
        public string windDirection { get; set; }
        public string windSpeed { get; set; }
        public string time { get; set; }
        public string date { get; set; }
    }

}
