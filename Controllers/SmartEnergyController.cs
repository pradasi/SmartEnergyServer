using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SmartEnergy.Services;
using SmartEnergy.ConfigClass;
using SmartEnergy.ContractClass;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public async Task<ActionResult> GetWeatherForecast()
        {
            string weatherData = await smartService.GetWeatherForecastUsingNet();
            if (weatherData.Contains("success"))
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("current-weather-report")]
        public async Task<ActionResult<CurrentWeatherData>> CurrentWeatherUsingNet()
        {
            var resposne = await smartService.GetCurrentWeatherUsingNet();
            if (resposne != null)
            {
                return Ok(resposne);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("predict/{modelName}")]
        public ActionResult<PredictedData> PredictModel(string modelName)
        {
            PredictedData predictedValue = smartService.Predict(modelName);
            if (predictedValue == null)
                return BadRequest();
            else
            {
                return Ok(predictedValue);
            }    
        }

        [HttpGet("today-weather")]
        public async Task<ActionResult<DailyWeather>> GetHourlyWeather()
        {
            var todayWeather = await smartService.HourlyWeatherData();
            if(todayWeather != null)
            {
                return Ok(todayWeather);
            }
            else
            {
                return BadRequest();
            }
            
        }

        [HttpGet("weekly-weather")]
        public async Task<ActionResult<IEnumerable<WeeklyWeather>>> GetWeeklyWeather()
        {
            var weeklyWeather = await smartService.WeeklyWeatherData();
            if(weeklyWeather == null)
            {
                BadRequest();
            }
            if(weeklyWeather.Count == 0)
            {
                return NotFound();
            }
            else
            {
                return Ok(weeklyWeather);
            } 
        }
    }
}
