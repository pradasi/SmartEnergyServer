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



    public class CurrentWeather
    {
        public string cloudCoverPhrase { get; set; }
        public string dayOfWeek { get; set; }
        public string dayOrNight { get; set; }
        public float pressureAltimeter { get; set; }
        public int relativeHumidity { get; set; }
        public int temperature { get; set; }
        public int temperatureDewPoint { get; set; }
        public DateTime validTimeLocal { get; set; }
        public float visibility { get; set; }
        public string windDirectionCardinal { get; set; }
        public int windSpeed { get; set; }

    }
}
