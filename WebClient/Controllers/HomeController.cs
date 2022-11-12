using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using WebClient.Diagnotics;
using WebClient.Models;

namespace WebClient.Controllers
{
    public class HomeController : Controller
    {
        private WebClientDiagnostics _diagnostics;
        private IHttpClientFactory _httpClientFactory;
        private readonly WebClientMetrics _metrics;

        public HomeController(WebClientDiagnostics diagnostics,IHttpClientFactory httpClientFactory, WebClientMetrics metrics)
        {
            _diagnostics = diagnostics;
            _httpClientFactory = httpClientFactory;
            _metrics = metrics;
        }

        public async Task<IActionResult> Index()
        {
            using (var activity = _diagnostics.OnHome("dotnetmalaga2022"))
            {
                //activity?.SetStatus(ActivityStatusCode.Error);
                //activity?.RecordException(new ArgumentNullException("test"));

                activity?.AddTag("webclient.call", "dotnetmalaga2022");  
                activity?.AddEvent(new ActivityEvent("webclient-call-event"));

                var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.GetFromJsonAsync<List<WeatherForecast>>("http://localhost:5050/WeatherForecast");
                response.ForEach(forecast => {
                    _metrics.RecordTemperature(forecast.TemperatureC);
                });

                _metrics.AddWeatherCall();
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
