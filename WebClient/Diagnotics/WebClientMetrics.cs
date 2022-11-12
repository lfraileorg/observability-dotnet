using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Threading;

namespace WebClient.Diagnotics
{
    public class WebClientMetrics
    {
        public const string WebClientMeterName = "WebClient";
        private Counter<int> WeatherCallsCounter { get; }  
        private Histogram<double> TemperaturesHistogram { get; }

        public string MetricName { get; }

        public WebClientMetrics(string meterName = WebClientMeterName)
        {
            var meter = new Meter(meterName);
            MetricName = meterName;

            WeatherCallsCounter = meter.CreateCounter<int>("weathercalls", "WeatherCalls");   
            TemperaturesHistogram = meter.CreateHistogram<double>("temperaturescelsius", "Temperatures Cº", "Temperatures distribution");
        }
                
        public void AddWeatherCall() => WeatherCallsCounter.Add(1);
        public void RecordTemperature(double temperature) => TemperaturesHistogram.Record(temperature);
    }
}
