using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Threading;

namespace WebClient.Diagnotics
{
    public class WebClientMetrics
    {
        private Counter<int> WeatherCallsCounter { get; }        
        private ObservableGauge<int> TotalCallsGauge { get; }
        private int _totalCalls = 0;

        ////Categories meters
        //private Counter<int> CategoriesAddedCounter { get; }
        //private Counter<int> CategoriesDeletedCounter { get; }
        //private Counter<int> CategoriesUpdatedCounter { get; }
        //private ObservableGauge<int> TotalCategoriesGauge { get; }
        //private int _totalCategories = 0;

        ////Order meters
        //private Histogram<double> OrdersPriceHistogram { get; }
        //private Histogram<int> NumberOfBooksPerOrderHistogram { get; }
        //private ObservableCounter<int> OrdersCanceledCounter { get; }
        //private int _ordersCanceled = 0;
        //private Counter<int> TotalOrdersCounter { get; }

        public string MetricName { get; }

        public WebClientMetrics(string meterName = "WebClient")
        {
            var meter = new Meter(meterName);
            MetricName = meterName;

            WeatherCallsCounter = meter.CreateCounter<int>("weather-calls", "Book");            
            TotalCallsGauge = meter.CreateObservableGauge("total-weather-calls", () => new[] { new Measurement<int>(_totalCalls) });

            //CategoriesAddedCounter = meter.CreateCounter<int>("categories-added", "Category");
            //CategoriesDeletedCounter = meter.CreateCounter<int>("categories-deleted", "Category");
            //CategoriesUpdatedCounter = meter.CreateCounter<int>("categories-updated", "Category");
            //TotalCategoriesGauge = meter.CreateObservableGauge("total-categories", () => _totalCategories);

            //OrdersPriceHistogram = meter.CreateHistogram<double>("orders-price", "Euros", "Price distribution of book orders");
            //NumberOfBooksPerOrderHistogram = meter.CreateHistogram<int>("orders-number-of-books", "Books", "Number of books per order");
            //OrdersCanceledCounter = meter.CreateObservableCounter("orders-canceled", () => _ordersCanceled);
            //TotalOrdersCounter = meter.CreateCounter<int>("total-orders", "Orders");
        }
                
        public void AddWeatherCall() => WeatherCallsCounter.Add(1);
        public void IncreaseTotalCalls() => _totalCalls++;

        ////Categories meters
        //public void AddCategory() => CategoriesAddedCounter.Add(1);
        //public void DeleteCategory() => CategoriesDeletedCounter.Add(1);
        //public void UpdateCategory() => CategoriesUpdatedCounter.Add(1);
        //public void IncreaseTotalCategories() => _totalCategories++;
        //public void DecreaseTotalCategories() => _totalCategories--;

        ////Orders meters
        //public void RecordOrderTotalPrice(double price) => OrdersPriceHistogram.Record(price);
        //public void RecordNumberOfBooks(int amount) => NumberOfBooksPerOrderHistogram.Record(amount);
        //public void IncreaseOrdersCanceled() => _ordersCanceled++;
        //public void IncreaseTotalOrders(string city) => TotalOrdersCounter.Add(1, KeyValuePair.Create<string, object>("City", city));

    }
}
