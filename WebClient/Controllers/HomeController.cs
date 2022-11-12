using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly WebClientMetrics _otelMetrics;

        public HomeController(WebClientDiagnostics diagnostics,IHttpClientFactory httpClientFactoryu, WebClientMetrics otelMetrics)
        {
            _diagnostics = diagnostics;
            _httpClientFactory = httpClientFactoryu;
            _otelMetrics = otelMetrics;
        }

        public async Task<IActionResult> Index()
        {
            using (var activity = _diagnostics.OnHome("with extra data"))
            {
                activity?.AddTag("webclient.call", "dotnetmalaga2022");  
                activity?.AddEvent(new ActivityEvent("webclient-call-event"));

                var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.GetFromJsonAsync<List<WeatherForecast>>("http://localhost:5050/WeatherForecast");
                response.ForEach(forecast => {
                    _otelMetrics.RecordTemperature(forecast.TemperatureC);
                });

                _otelMetrics.AddWeatherCall();
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
