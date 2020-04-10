using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using SmartEnergy.ConfigClass;
using SmartEnergy.ContractClass;
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
                using (StreamReader reader =  process.StandardOutput)
                {
                    string result =  reader.ReadToEnd();
                    return result;
                }
            }
        }

        public CurrentWeatherData GetCurrentWeather()
        {
            return JsonConvert.DeserializeObject<CurrentWeatherData>(PythonExecuter(pythonFile.currentWeatherdata));
        }

        public PredictedData Predict(string modelName)
        {
            if(modelName.ToLower() == forSolar)
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

        public string GetWeatherForecast()
        {
            return PythonExecuter(pythonFile.weatherReport);
        }

        public DailyWeather HourlyWeatherData()
        {
            return JsonConvert.DeserializeObject<DailyWeather>(PythonExecuter(pythonFile.hourlyWeatherData));
        }

        public IEnumerable<WeeklyWeather> WeeklyWeatherData()
        {
            return JsonConvert.DeserializeObject<IEnumerable<WeeklyWeather>>(PythonExecuter(pythonFile.weeklyWeatherData));
        }
    }
}
