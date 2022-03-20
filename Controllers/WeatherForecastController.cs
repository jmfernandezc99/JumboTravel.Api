using Microsoft.AspNetCore.Mvc;

namespace JumboTravel.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet("Hola")]
        public async Task<IActionResult> Test([FromQuery] string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                return Ok(message + "Prueba");
            } else
            {
                return NotFound(message);
            }
        }
    }
}