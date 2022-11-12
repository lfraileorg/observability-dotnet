using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Threading;

namespace WebClient.Diagnotics
{

    public partial class WebClientDiagnostics
    {
        public const string ActivitySourceName = "HomeModule";
        private ILogger _logger;
        private static ActivitySource activitySource = new ActivitySource(ActivitySourceName, version: "ver1.0");


        [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "This is the message with {data}")]
        private partial void OnHomeMessage(string data);

        public WebClientDiagnostics(ILoggerFactory loggerFactory)
        {
            _ = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = loggerFactory.CreateLogger("WebClientDiagnostics");
        }

        public Activity OnHome(string data)
        {
            WebClientCounterSets.Instance.IncrementHomeIndex();
            
            this.OnHomeMessage(data);          

            return activitySource.StartActivity("HomeIndex");
        }
    }

    class WebClientCounterSets
        :EventSource
    {

        public static WebClientCounterSets Instance = new WebClientCounterSets();

        private IncrementingPollingCounter _homePerSecondCounters;
        private long _homePerSecondValue;

        public WebClientCounterSets()
            :base("WebClientCounters", EventSourceSettings.EtwSelfDescribingEventFormat)
        {
            _homePerSecondCounters = new IncrementingPollingCounter("HomePerSecond", this, () => _homePerSecondValue)
            {
                DisplayName = "HomePerSecond",
                DisplayRateTimeScale = TimeSpan.FromSeconds(1)
            };
        }



        [Event(1, Level = EventLevel.Informational)]
        public void IncrementHomeIndex()
        {
            Interlocked.Increment(ref _homePerSecondValue);

            WriteEvent(1);
        }
    }
}
