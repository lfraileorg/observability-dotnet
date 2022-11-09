using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SomeService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private static ActivitySource _activitySource = new ActivitySource("WeatherService", version: "ver1.0");
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly TelemetryClient _telemetryClient;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, TelemetryClient telemetryClient)
        {
            _logger = logger;
            _telemetryClient = telemetryClient;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            using (var activity = _activitySource.StartActivity("GetForecast"))
            {
                _telemetryClient.TrackEvent("MyEvent");
                activity.AddTag("weatherforecast", "value");
                activity.AddEvent(new ActivityEvent("MyEventOtl"));

                var rng = new Random();
                return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                })
                .ToArray();
            }
        }
    }
}
