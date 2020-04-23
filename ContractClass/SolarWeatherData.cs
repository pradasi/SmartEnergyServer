using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartEnergy.ContractClass
{
    public class SolarWeatherData
    {
        public string version { get; set; }
        public List<SolarData> data { get; set; }
    }

    public class SolarData
    {
        public string parameter { get; set; }
        public List<Coordinate> coordinates { get; set; }
    }

    public class Coordinate
    {
        public float lat { get; set; }
        public float lon { get; set; }
        public List<Date> dates { get; set; }
    }

    public class Date
    {
        public string date { get; set; }
        public string value { get; set; }
    }

    public class SolarCsvFormat
    {
        public string Elevation { get; set; }
        public string Azimuth { get; set; }
        public string Temperature { get; set; }
        public string Humidity { get; set; }
        public string Pressure { get; set; }
        public string Irradiation { get; set; }
        public string ActualIrradiation { get; set; }
    }



    public class WindWeatherData
    {
        public List<int> temperature { get; set; }
        public List<string> windDirectionCardinal { get; set; }
        public List<int> relativeHumidity { get; set; }
        public List<float> pressureMeanSeaLevel { get; set; }
        public List<int> windSpeed { get; set; }
    }

    public class WindCsvFormat
    {
        public int Temperature { get; set; }
        public string WindDirection { get; set; }
        public int Humidity { get; set; }
        public float Pressure { get; set; }
        public int WindSpeed { get; set; }
    }

}
