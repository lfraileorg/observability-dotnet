using Serilog;
using Serilog.Sinks.Grafana.Loki;
using Serilog.Enrichers.Span;
using OpenTelemetry.Resources;
using WebClient.Diagnotics;
using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

Serilog.Log.Logger = new LoggerConfiguration()
     .WriteTo.GrafanaLoki("http://localhost:3100", outputTemplate: @"[{Timestamp:o} {Level:u3}] {TraceId:l} {Message:lj}{NewLine}{Exception}")
     .WriteTo.Console(Serilog.Events.LogEventLevel.Information)
     .WriteTo.ApplicationInsights(builder.Configuration["ApplicationInsights:ConnectionString"], TelemetryConverter.Traces, Serilog.Events.LogEventLevel.Warning)
     .Enrich.FromLogContext()
     .Enrich.WithSpan()
     .CreateLogger();

builder.Logging.ClearProviders();
builder.Host.UseSerilog();

builder.Services.AddHttpClient();
builder.Services.AddControllersWithViews();

builder.Services.AddOpenTelemetryTracing(builderTelemetry =>
{

    builderTelemetry.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("WebClient", serviceVersion: "ver1.0"))
        .AddAspNetCoreInstrumentation(options =>
        {
            //options.Filter = (httpContext) =>
            //{
            //    return httpContext.Request.Method == HttpMethods.Get && httpContext.Request.Path.Value == "/";
            //};
        })
        .AddHttpClientInstrumentation(options =>
        {
            //options.FilterHttpRequestMessage = (httpContext) =>
            //{
            //    return httpContext.Method.Method == HttpMethods.Get;
            //};
        })
        .AddSource("HomeModule")
        .AddAzureMonitorTraceExporter(o =>
        {
            o.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
        })
        .AddJaegerExporter();

});

builder.Services.AddOpenTelemetryMetrics(options =>
{
    options.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("WebClient"))
    .AddRuntimeInstrumentation()
    .AddHttpClientInstrumentation()
    .AddAspNetCoreInstrumentation()
    .AddMeter(WebClientMetrics.WebClientMeterName)
    .AddAzureMonitorMetricExporter(o =>
    {
        o.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
    })
    .AddView(
        instrumentName: "temperatures-celsius",
        new ExplicitBucketHistogramConfiguration { Boundaries = new double[] { -10, 0, 10, 30, 50 } })

    .AddPrometheusExporter();
});
builder.Services.AddSingleton<WebClientDiagnostics>();
builder.Services.AddSingleton<WebClientMetrics>();

//builder.Services.AddApplicationInsightsTelemetry(o =>
//{
//    o.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
//});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});
app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.Run();