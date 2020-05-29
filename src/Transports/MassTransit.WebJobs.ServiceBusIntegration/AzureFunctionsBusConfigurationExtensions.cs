namespace MassTransit
{
    using System;
    using Azure.ServiceBus.Core;
    using ExtensionsDependencyInjectionIntegration;
    using Microsoft.ApplicationInsights.DependencyCollector;
    using Microsoft.Azure.WebJobs.ServiceBus;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using WebJobs.ServiceBusIntegration;


    public static class AzureFunctionsBusConfigurationExtensions
    {
        /// <summary>
        /// Add the Azure Function support for MassTransit, which uses Azure Service Bus, and configures
        /// <see cref="IMessageReceiver" /> for use by functions to handle messages. Uses <see cref="ServiceBusOptions.ConnectionString" />
        /// to connect to Azure Service Bus.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure">
        /// Configure via <see cref="DependencyInjectionRegistrationExtensions.AddMassTransit" />, to configure consumers, etc.
        /// </param>
        /// <param name="configureBus">Optional, configure the service bus settings</param>
        /// <returns></returns>
        public static IServiceCollection AddMassTransitForAzureFunctions(this IServiceCollection services, Action<IServiceCollectionConfigurator> configure,
            Action<IServiceBusBusFactoryConfigurator> configureBus = default)
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
                        var options = context.Container.GetRequiredService<IOptions<ServiceBusOptions>>();

                        options.Value.MessageHandlerOptions.AutoComplete = true;

                        cfg.Host(options.Value.ConnectionString);
                        cfg.UseServiceBusMessageScheduler();

                        configureBus?.Invoke(cfg);
                    });
                });

            return services;
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
