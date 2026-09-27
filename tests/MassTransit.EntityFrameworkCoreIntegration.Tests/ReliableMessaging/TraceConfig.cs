namespace MassTransit.EntityFrameworkCoreIntegration.Tests.ReliableMessaging
{
    using System;
    using System.Diagnostics;
    using OpenTelemetry;
    using OpenTelemetry.Resources;
    using OpenTelemetry.Trace;


    public static class TraceConfig
    {
        public static ActivitySource Source => Cached.Source.Value;

        public static TracerProvider CreateTraceProvider(string serviceName)
        {
            return Sdk.CreateTracerProviderBuilder()
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
                .AddSource("MassTransit")
                .AddSource("UnitTests")
                .Build();
        }


        static class Cached
        {
            internal static readonly Lazy<ActivitySource> Source = new Lazy<ActivitySource>(() => new ActivitySource("UnitTests"));
        }
    }
}
