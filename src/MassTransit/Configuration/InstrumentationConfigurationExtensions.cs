namespace MassTransit
{
    using System;
    using Logging;
    using Monitoring;


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
            var options = new InstrumentationOptions();
            var configureDefault = new ConfigureDefaultInstrumentationOptions();

            configureDefault.Configure(options);
            configureOptions?.Invoke(options);

            if (!string.IsNullOrWhiteSpace(serviceName))
                options.ServiceName = serviceName;

            LogContextInstrumentationExtensions.TryConfigure(options);
        }
    }
}
