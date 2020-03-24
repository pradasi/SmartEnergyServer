using System;

namespace SmartEnergy.ContractClass
{
    public class WeatherData
    {
        public string cloudPhrase { get; set; }
        public string dayOfWeek { get; set; }
        public string dayOrNight { get; set; }
        public float visibility { get; set; }
        public float pressure { get; set; }
        public int relativeHumidity { get; set; }
        public int temperature { get; set; }
        public int dewPoint { get; set; }
        public string windDirection { get; set; }
        public int windSpeed { get; set; }
        public string time { get; set; }
        public string date { get; set; }
    }

}
