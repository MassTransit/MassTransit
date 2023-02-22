namespace MassTransit
{
    using System;
    using Azure.Identity;
    using Azure.Messaging.ServiceBus;
    using AzureServiceBusTransport;
    using Microsoft.ApplicationInsights.DependencyCollector;
    using Microsoft.Azure.WebJobs.ServiceBus;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;


    public static class AzureFunctionsBusConfigurationExtensions
    {
        /// <summary>
        /// Add the Azure Function support for MassTransit, which uses Azure Service Bus, and configures
        /// <see cref="MassTransit.IMessageReceiver" /> for use by functions to handle messages.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure">
        /// Configure via <see cref="DependencyInjectionRegistrationExtensions.AddMassTransit" />, to configure consumers, etc.
        /// </param>
        /// <param name="connectionStringConfigurationKey">
        /// Optional, the name of the configuration value for the connection string.
        /// </param>
        /// <param name="configureBus">Optional, the configuration callback for the bus factory</param>
        /// <returns></returns>
        public static IServiceCollection AddMassTransitForAzureFunctions(this IServiceCollection services,
            Action<IBusRegistrationConfigurator> configure,
            string connectionStringConfigurationKey = "ServiceBus",
            Action<IBusRegistrationContext, IServiceBusBusFactoryConfigurator> configureBus = default)
        {
            ConfigureApplicationInsights(services);

            services
                .AddSingleton<IMessageReceiver, MessageReceiver>()
                .AddSingleton<IAsyncBusHandle, AsyncBusHandle>()
                .AddMassTransit(x =>
                {
                    configure?.Invoke(x);

                    x.UsingAzureServiceBus((context, cfg) =>
                    {
                        var options = context.GetRequiredService<IOptions<ServiceBusOptions>>();

                        options.Value.AutoCompleteMessages = true;

                        var config = context.GetRequiredService<IConfiguration>();
                        var connectionString = config[connectionStringConfigurationKey];

                        if (string.IsNullOrWhiteSpace(connectionString))
                        {
                            var ns = config["ServiceBusConnection:fullyQualifiedNamespace"]
                                ?? config[$"{connectionStringConfigurationKey}:fullyQualifiedNamespace"];
                            if (string.IsNullOrWhiteSpace(ns))
                            {
                                throw new ArgumentNullException(connectionStringConfigurationKey,
                                    "The service bus connection string or namespace was not configured");
                            }

                            connectionString = $"Endpoint=sb://{ns}";
                        }

                        cfg.Host(connectionString, h =>
                        {
                            if (IsMissingCredentials(connectionString))
                                h.TokenCredential = new DefaultAzureCredential();
                        });

                        cfg.UseServiceBusMessageScheduler();

                        configureBus?.Invoke(context, cfg);
                    });
                });

            services.RemoveMassTransitHostedService();

            return services;
        }

        static bool IsMissingCredentials(string connectionString)
        {
            if (!connectionString.Contains("=")) return true;

            var properties = ServiceBusConnectionStringProperties.Parse(connectionString);

            return (string.IsNullOrWhiteSpace(properties.SharedAccessKeyName) || string.IsNullOrWhiteSpace(properties.SharedAccessKey))
                && string.IsNullOrWhiteSpace(properties.SharedAccessSignature);
        }

        static void ConfigureApplicationInsights(IServiceCollection services)
        {
            services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((module, o) =>
            {
                module.IncludeDiagnosticSourceActivities.Add("MassTransit");
            });
        }
    }
}
