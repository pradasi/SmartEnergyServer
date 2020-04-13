using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SmartEnergy.ConfigClass;
using SmartEnergy.ContractClass;
using System;
using System.Net;
using System.Text;
using CsvHelper;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

namespace SmartEnergy.Services
{
    public class SmartService
    {
        private readonly string prompt;
        private readonly string directory;
        private PythonFile pythonFile;
        private DirectorySetup directorySetup;
        private string forSolar = "solar";
        private string forWind = "wind";
        public SmartService(PythonFile pyFile, DirectorySetup dirSetup)
        {
            pythonFile = pyFile;
            directorySetup = dirSetup;
            prompt = directorySetup.prompt;
            directory = directorySetup.directory;
        }

        private string PythonExecuter(string fileName)
        {
            string cmd = prompt;
            string args = directory + fileName;
            ProcessStartInfo pythonInfo = new ProcessStartInfo();
            pythonInfo.FileName = cmd;
            pythonInfo.Arguments = args;
            pythonInfo.WindowStyle = ProcessWindowStyle.Hidden;
            pythonInfo.CreateNoWindow = false;
            pythonInfo.UseShellExecute = false;
            pythonInfo.RedirectStandardOutput = true;
            using (Process process = Process.Start(pythonInfo))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    return result;
                }
            }
        }

        private async Task<string> CallHttClient(string url)
        {
            HttpClient httpClient = new HttpClient();
            var responseData = httpClient.GetAsync(url);

            var data = await responseData.Result.Content.ReadAsStringAsync();

            return data;

        }

        private async Task<string> CallMeteoMatics()
        {
            string credentials = "cmrit_kumar:aM3OVy9uYz7Ei";
            DateTime dt = DateTime.Now;
            string datetime = dt.ToString("yyyy-MM-ddTHH:mm:ss+00:00");

            string formattedUrl = $"https://api.meteomatics.com/" + datetime + "P2D:PT1H/sun_elevation:d,sun_azimuth:d,t_2m:C,relative_humidity_1000hPa:p,sfc_pressure:hPa,global_rad:W/12.97210,77.59330/json";
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(formattedUrl);
            request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials)));
            request.PreAuthenticate = true;
            WebResponse response = await request.GetResponseAsync();

            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                string responseText = reader.ReadToEnd();
                return responseText;
            }
        }

        private bool SplitDataWriteToCsv(SolarWeatherData responseData, WindWeatherData windData)
        {
            try
            {
                #region solar
                List<SolarCsvFormat> csvData = new List<SolarCsvFormat>();

                var elevation = responseData.data[0].coordinates[0].dates.Skip(1).Take(24).ToList().Select(data => data.value).ToList();
                var azimuth = responseData.data[1].coordinates[0].dates.Skip(1).Take(24).ToList().Select(data => data.value).ToList();
                var temperature = responseData.data[1].coordinates[0].dates.Skip(1).Take(24).ToList().Select(data => data.value).ToList();
                var humidity = responseData.data[1].coordinates[0].dates.Skip(1).Take(24).ToList().Select(data => data.value).ToList();
                var pressure = responseData.data[1].coordinates[0].dates.Skip(1).Take(24).ToList().Select(data => data.value).ToList();
                var irrad = responseData.data[1].coordinates[0].dates.Skip(1).Take(24).ToList().Select(data => data.value).ToList();
                var actualIrrad = responseData.data[1].coordinates[0].dates.Take(24).ToList().Select(data => data.value).ToList();
                var Time = responseData.data[0].coordinates[0].dates.Skip(1).Take(24).ToList().Select(data => data.date).ToList();
                for (int timeIndex = 0; timeIndex < Time.Count; timeIndex++)
                {
                    Time[timeIndex] = Time[timeIndex].Split("T")[1].Split("Z")[0];
                }

                for (int csvIndex = 0; csvIndex < elevation.Count; csvIndex++)
                {
                    csvData.Add(
                        new SolarCsvFormat
                        {
                            Time = Time[csvIndex],
                            Elevation = elevation[csvIndex],
                            Azimuth = azimuth[csvIndex],
                            Temperature = temperature[csvIndex],
                            Humidity = humidity[csvIndex],
                            Pressure = pressure[csvIndex],
                            Irradiation = irrad[csvIndex],
                            ActualIrradiation = actualIrrad[csvIndex]

                        }
                    );
                }

                string solarPath = directorySetup.directory + pythonFile.solarCsv;
                using (var writer = new StreamWriter(solarPath))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(csvData);
                }

                #endregion

                #region wind
                List<WindCsvFormat> windCsvData = new List<WindCsvFormat>();

                var time = Time;
                var wtemperature = windData.temperature.Take(24).ToList();
                var wDewPoint = windData.temperatureDewPoint.Take(24).ToList();
                var wDirection = windData.windDirectionCardinal.Take(24).ToList();
                var wHumidity = windData.relativeHumidity.Take(24).ToList();
                var wPressure = windData.pressureMeanSeaLevel.Take(24).ToList();
                var windSpeed = windData.windSpeed.Take(24).ToList();


                for (int csvIndex = 0; csvIndex < wtemperature.Count; csvIndex++)
                {
                    windCsvData.Add(
                        new WindCsvFormat
                        {

                            DateTime = Time[csvIndex],
                            Temperature = wtemperature[csvIndex],
                            DewPoint = wDewPoint[csvIndex],
                            WindDirection = wDirection[csvIndex],
                            Humidity = wHumidity[csvIndex],
                            Pressure = wPressure[csvIndex],
                            WindSpeed = windSpeed[csvIndex],

                        }
                    );
                }

                string windPath = directorySetup.directory + pythonFile.windCsv;

                using (var writer = new StreamWriter(windPath))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(windCsvData);
                }

                #endregion
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<string> GetWeatherForecastUsingNet()
        {
            SolarWeatherData responseData = JsonConvert.DeserializeObject<SolarWeatherData>(await CallMeteoMatics());

            string Windurl = $"https://api.weather.com/v3/wx/forecast/hourly/15day?apiKey=6532d6454b8aa370768e63d6ba5a832e&geocode=12.97%2C77.598&units=e&language=en-US&format=json";


            WindWeatherData windData = JsonConvert.DeserializeObject<WindWeatherData>(await CallHttClient(Windurl));

            bool writtenSuccessful = SplitDataWriteToCsv(responseData, windData);

            if (writtenSuccessful)
                return "success";
            else
                return "failure";
        }

        public async Task<CurrentWeatherData> GetCurrentWeatherUsingNet()
        {
            string url = $"https://api.weather.com/v3/wx/observations/current?apiKey=6532d6454b8aa370768e63d6ba5a832e&geocode=12.97%2C77.598&units=e&language=en-US&format=json";

            CurrentWeather currentWeatherDataJson = JsonConvert.DeserializeObject<CurrentWeather>(await CallHttClient(url));
            if (currentWeatherDataJson != null)
            {
                CurrentWeatherData currentWeatherData = ExtractCurrentWeather(currentWeatherDataJson);

                return currentWeatherData;
            }
            else
            {
                return null;
            }
        }

        private CurrentWeatherData ExtractCurrentWeather(CurrentWeather currentWeatherDataJson)
        {
            CurrentWeatherData currentWeatherData = new CurrentWeatherData();

            currentWeatherData.cloudPhrase = currentWeatherDataJson.cloudCoverPhrase;
            currentWeatherData.dayOfWeek = currentWeatherDataJson.dayOfWeek;
            currentWeatherData.dayOrNight = currentWeatherDataJson.dayOrNight;
            currentWeatherData.visibility = currentWeatherDataJson.visibility.ToString() + "%";
            currentWeatherData.pressure = currentWeatherDataJson.pressureAltimeter.ToString() + " in";
            currentWeatherData.temperature = currentWeatherDataJson.temperature.ToString() + " \u2109";
            currentWeatherData.relativeHumidity = currentWeatherDataJson.relativeHumidity.ToString() + "%";
            currentWeatherData.windDirection = currentWeatherDataJson.windDirectionCardinal;
            currentWeatherData.dewPoint = currentWeatherDataJson.temperatureDewPoint.ToString() + " \u2109";
            currentWeatherData.windSpeed = currentWeatherDataJson.windSpeed.ToString() + " mph";
            currentWeatherData.time = currentWeatherDataJson.validTimeLocal.ToString("HH:mm:ss");
            currentWeatherData.date = currentWeatherDataJson.validTimeLocal.ToString("dd MMM yyyy");

            return currentWeatherData;

        }

        public PredictedData Predict(string modelName)
        {
            if (modelName.ToLower() == forSolar)
            {
                return JsonConvert.DeserializeObject<PredictedData>(PredictSolarEnergy());
            }
            if (modelName.ToLower() == forWind)
            {
                return JsonConvert.DeserializeObject<PredictedData>(PredictWind());
            }
            else
            {
                return null;
            }
        }

        private string PredictSolarEnergy()
        {
            return PythonExecuter(pythonFile.solarModel);
        }

        private string PredictWind()
        {
            return PythonExecuter(pythonFile.windModel);
        }

        public async Task<List<WeeklyWeather>> WeeklyWeatherData()
        {
            string urlForWeather = $"https://api.weather.com/v3/wx/forecast/daily/15day?apiKey=6532d6454b8aa370768e63d6ba5a832e&geocode=13.20%2C77.71&language=en-US&units=e&format=json";

            WeeklyJson weeklyJson = JsonConvert.DeserializeObject<WeeklyJson>(await CallHttClient(urlForWeather));

            List<WeeklyWeather> weeklyWeathers = ExtractWeeklyWeathers(weeklyJson);

            return weeklyWeathers;
        }

        private List<WeeklyWeather> ExtractWeeklyWeathers(WeeklyJson weeklyJson)
        {
            List<WeeklyWeather> weeklyWeathers = new List<WeeklyWeather>();
            try
            {
                for (int numberofDays = 1; numberofDays <= 7; numberofDays++)
                {
                    weeklyWeathers.Add(
                        new WeeklyWeather
                        {
                            date = weeklyJson.validTimeLocal[numberofDays].ToString("dd MMM yyyy"),
                            day = weeklyJson.dayOfWeek[numberofDays],
                            maxTemperature = weeklyJson.temperatureMax[numberofDays].ToString() + " \u2109",
                            minTemperature = weeklyJson.temperatureMin[numberofDays].ToString() + " \u2109",
                            weatherCondition = weeklyJson.narrative[numberofDays].Split(".")[0]
                        }
                    );
                }
                return weeklyWeathers;
            }
            catch (Exception)
            {
                return null;
            }

        }

        public async Task<DailyWeather> HourlyWeatherData()
        {
            string urlForWeather = "https://api.weather.com/v3/wx/forecast/hourly/15day?apiKey=6532d6454b8aa370768e63d6ba5a832e&geocode=12.97%2C77.598&units=e&language=en-US&format=json";

            HourlyWeather hourlyWeathers =  JsonConvert.DeserializeObject<HourlyWeather>(await CallHttClient(urlForWeather));

            DailyWeather dailyWeather = ExtractDailyWeather(hourlyWeathers);

            return dailyWeather;
        }

        private DailyWeather ExtractDailyWeather(HourlyWeather hourlyWeathers)
        {
           
            try
            {
                DailyWeather dailyWeather = new DailyWeather();

                string today = hourlyWeathers.dayOfWeek[0];

                int lastIndex = hourlyWeathers.dayOfWeek.LastIndexOf(today, 24);

                int iterable = (lastIndex / 3) + 1;

                dailyWeather.date = hourlyWeathers.validTimeLocal[0].ToString("dd MMM yyyy");
                dailyWeather.day = today;

                List<HourlyWeatherData> hourlyWeatherData = new List<HourlyWeatherData>();

                for (int hourIndex = 0; hourIndex < lastIndex; hourIndex+=3)
                {
                    hourlyWeatherData.Add(
                            new HourlyWeatherData
                            {
                                pressure = hourlyWeathers.pressureMeanSeaLevel[hourIndex].ToString() + " in",
                                temperature = hourlyWeathers.temperature[hourIndex].ToString() + " \u2109",
                                time = hourlyWeathers.validTimeLocal[hourIndex].ToString("HH:mm:ss"),
                                windSpeed = hourlyWeathers.windSpeed[hourIndex].ToString() + " mph",
                                weatherCondition = hourlyWeathers.wxPhraseLong[hourIndex]
                            }
                        );
                }
                
                dailyWeather.data = hourlyWeatherData;

                return dailyWeather;
            }
            catch (Exception)
            {
                return null;
            }

        }
    }
}
