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
        public SmartService smartService;

        public SmartEnergyController(IOptions<PythonFile> pyFiles, IOptions<DirectorySetup> dirSetup, IOptions<Credentials> credentials )
        {
            smartService = new SmartService(pyFiles.Value, dirSetup.Value, credentials.Value);
        }

        [HttpGet("init")]
        public ActionResult InitTF()
        {
            string status = smartService.InitTf();
            if(status.Contains("success"))
            {
                return Accepted();
            }
            else
            {
                return BadRequest();
            }

        }

        [HttpGet("weather-forecast-solar")]
        public async Task<ActionResult> GetWeatherForecastSolar()
        {
            string weatherData = await smartService.GetWeatherForecastSolar();
            if (weatherData.Contains("success"))
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("weather-forecast-wind")]
        public async Task<ActionResult> GetWeatherForecastWind()
        {
            string weatherData = await smartService.GetWeatherForecastWind();
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
        public async Task<ActionResult<CurrentWeatherData>> CurrentWeather()
        {
            var resposne = await smartService.GetCurrentWeather();
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

        [HttpGet("calculate-power")]
        public ActionResult<PredictedData> GetCalculatedPower()
        {
            PredictedData predictedValue = smartService.CalculatePower();
            if (predictedValue != null)
            {
                return Ok(predictedValue);
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
