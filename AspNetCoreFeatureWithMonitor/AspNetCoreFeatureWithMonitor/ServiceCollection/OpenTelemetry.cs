using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace AspNetCoreFeatureWithMonitor.ServiceCollection;

public static class OpenTelemetry
{
    public static IServiceCollection AddCustomOpenTelemetry(this IServiceCollection service)
    {
        service.AddOpenTelemetry().WithMetrics(opt =>
        {
            opt.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("AspNetCoreFeatureWithMonitor"))
                .AddMeter("AspNetCoreFeatureWithMonitor")
                .AddAspNetCoreInstrumentation()
                .AddConsoleExporter()
                .AddPrometheusExporter();
        });
        return service;
    }
}