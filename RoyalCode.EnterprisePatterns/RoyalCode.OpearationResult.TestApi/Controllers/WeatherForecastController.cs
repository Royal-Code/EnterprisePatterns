using Microsoft.AspNetCore.Mvc;
using RoyalCode.OpearationResult.TestApi.Application.WeatherForecasts;

namespace RoyalCode.OpearationResult.TestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly WeatherForecastService service;

        public WeatherForecastController(WeatherForecastService service)
        {
            this.service = service;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public ActionResult<IEnumerable<WeatherForecast>> Get()
        {
            return service.Get().ToResult();
        }
    }
}