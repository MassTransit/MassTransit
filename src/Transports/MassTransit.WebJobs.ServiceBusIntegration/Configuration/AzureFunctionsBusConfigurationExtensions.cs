namespace MassTransit
{
    using System;
    using Azure.Identity;
    using Azure.Messaging.ServiceBus;
    using Microsoft.ApplicationInsights.DependencyCollector;
    using Microsoft.Azure.WebJobs.ServiceBus;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using ServiceBusIntegration;


    public static class AzureFunctionsBusConfigurationExtensions
    {
        /// <summary>
        /// Add the Azure Function support for MassTransit, which uses Azure Service Bus, and configures
        /// <see cref="IMessageReceiver" /> for use by functions to handle messages.
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
                            throw new ArgumentNullException(connectionStringConfigurationKey, "A connection string must be used for Azure Functions.");

                        cfg.Host(connectionString, h =>
                        {
                            if (IsMissingCredentials(connectionString))
                                h.TokenCredential = new ManagedIdentityCredential();
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
