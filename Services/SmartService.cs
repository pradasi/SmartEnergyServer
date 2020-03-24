using System.Diagnostics;
using System.IO;
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

        public WeatherData GetCurrentWeather()
        {
            return JsonConvert.DeserializeObject<WeatherData>(PythonExecuter(pythonFile.currentWeatherdata));
        }

        public string PredictSolarEnergy()
        {
            return PythonExecuter(pythonFile.solarModel);
        }

        public string PredictWind()
        {
            return PythonExecuter(pythonFile.windModel);
        }

        public string GetWeatherForecast()
        {
            return PythonExecuter(pythonFile.weatherReport);

        }

    }
}
