using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddControllers();

services.AddOpenTelemetryTracing(telemetryBuilder =>
{
    telemetryBuilder.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("SomeService", serviceVersion: "ver1.0"))
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddSource("WeatherService")
        .AddAzureMonitorTraceExporter(o =>
        {
            o.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
        })
        .AddJaegerExporter()
        ;
});

//services.AddApplicationInsightsTelemetry(o =>
//{
//    o.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
//});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();