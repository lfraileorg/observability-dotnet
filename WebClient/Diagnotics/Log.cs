using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Threading;

namespace WebClient.Diagnotics
{

    public class WebClientDiagnostics
    {
        private ILogger _logger;

        private static ActivitySource activitySource = new ActivitySource("HomeModule", version: "ver1.0");



        public WebClientDiagnostics(ILoggerFactory loggerFactory)
        {
            _ = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));

            _logger = loggerFactory.CreateLogger("WebClient");
        }

        public Activity OnHome(string data)
        {
            WebClientCounterSets.Instance.IncrementHomeIndex();

            Log.HomeMessage(_logger, data);

            return activitySource.StartActivity("HomeIndex");
        }
    }
    public static class Log
    {
        public static void HomeMessage(ILogger logger, string message)
        {
            _homeMessage(logger, message, null);
        }

        private static Action<ILogger, string, Exception> _homeMessage = LoggerMessage.Define<string>(
                LogLevel.Warning,
                HomeId,
                "this is the warning message with data {data}");

        private static EventId HomeId = new EventId(100, nameof(HomeId));
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
