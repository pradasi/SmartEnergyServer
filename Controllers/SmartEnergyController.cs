using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SmartEnergy.Services;
using SmartEnergy.ConfigClass;
using SmartEnergy.ContractClass;

namespace SmartEnergy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SmartEnergyController : ControllerBase
    {
        public PythonFile pythonFiles;
        public DirectorySetup directorySetup;
        public SmartService smartService;

        public SmartEnergyController(IOptions<PythonFile> pyFiles, IOptions<DirectorySetup> dirSetup )
        {
            pythonFiles = pyFiles.Value;
            directorySetup = dirSetup.Value;
            smartService = new SmartService(pythonFiles, directorySetup);
        }

        [HttpGet("weather-forecast")]
        public ActionResult<string> GetAction()
        {
            string weatherData = smartService.GetWeatherForecast();
            if (weatherData.Contains("success"))
            {
                return Ok("Upadted Weather Forecast Successfully");
            }
            else
            {
                return BadRequest("Could Not Upadte File");
            }
        }

        [HttpGet("current-weather-report")]
        public ActionResult<WeatherData> CurrentWeather()
        {
            return smartService.GetCurrentWeather();
        }


        [HttpGet("predict/{modelName}")]
        public IActionResult PredictModel(string modelName)
        {
            return Ok(smartService.Predict(modelName));
        }
    }
}
