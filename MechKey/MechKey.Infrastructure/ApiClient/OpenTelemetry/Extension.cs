using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Trace;

namespace Infrastructure.ApiClient.OpenTelemetry
{
    public static class Extension
    {
        public static IHostApplicationBuilder AddOpenTelemetry(this IHostApplicationBuilder builder)
        {

            builder.Services.AddOpenTelemetry()
               .WithTracing(tracing =>
               {
                   tracing
                       .AddAspNetCoreInstrumentation()
                       .AddHttpClientInstrumentation()
                       .AddAspNetCoreInstrumentation()
                       .AddConsoleExporter();
               });

            return builder;
        }
    }
}
