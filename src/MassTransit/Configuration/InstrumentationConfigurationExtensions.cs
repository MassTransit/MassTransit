namespace MassTransit
{
    using System;
    using Metadata;
    using Monitoring;
    using Monitoring.Configuration;


    public static class InstrumentationConfigurationExtensions
    {
        /// <summary>
        /// Enables instrumentation using the built-in .NET Meter class, which can be collected by OpenTelemetry.
        /// See https://docs.microsoft.com/en-us/dotnet/core/diagnostics/metrics for details.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configureOptions"></param>
        /// <param name="serviceName">
        /// The service name for metrics reporting, defaults to the current process main module filename
        /// </param>
        public static void UseInstrumentation(this IBusFactoryConfigurator configurator, Action<InstrumentationOptions> configureOptions = null,
            string serviceName = default)
        {
            var options = InstrumentationOptions.CreateDefault();

            configureOptions?.Invoke(options);

            Instrumentation.TryConfigure(GetServiceName(serviceName), options);

            configurator.ConnectConsumerConfigurationObserver(new InstrumentConsumerConfigurationObserver());
            configurator.ConnectHandlerConfigurationObserver(new InstrumentHandlerConfigurationObserver());
            configurator.ConnectSagaConfigurationObserver(new InstrumentSagaConfigurationObserver());
            configurator.ConnectActivityConfigurationObserver(new InstrumentActivityConfigurationObserver());
            configurator.ConnectEndpointConfigurationObserver(new InstrumentReceiveEndpointConfiguratorObserver());
            configurator.ConnectBusObserver(new InstrumentBusObserver());
        }

        static string GetServiceName(string serviceName)
        {
            return string.IsNullOrWhiteSpace(serviceName)
                ? HostMetadataCache.Host.ProcessName
                : serviceName;
        }
    }
}
