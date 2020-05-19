namespace MassTransit
{
    using System;
    using Azure.ServiceBus.Core;
    using Azure.ServiceBus.Core.Configuration;
    using Azure.ServiceBus.Core.Configurators;
    using ExtensionsDependencyInjectionIntegration;
    using MassTransit;
    using Microsoft.ApplicationInsights.DependencyCollector;
    using Microsoft.Azure.WebJobs.ServiceBus;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using WebJobs.ServiceBusIntegration;


    public static class AzureFunctionsBusConfigurationExtensions
    {
        /// <summary>
        /// Add the Azure Function support for MassTransit, which uses Azure Service Bus, and configures
        /// <see cref="IMessageReceiver"/> for use by functions to handle messages. Uses <see cref="ServiceBusOptions.ConnectionString"/>
        /// to connect to Azure Service Bus.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure">Configure via <see cref="DependencyInjectionRegistrationExtensions.AddMassTransit"/>, to configure consumers, etc.</param>
        /// <param name="configureBus">Optional, configure the service bus settings</param>
        /// <returns></returns>
        public static IServiceCollection AddMassTransitForAzureFunctions(this IServiceCollection services, Action<IServiceCollectionConfigurator> configure,
            Action<IServiceBusBusFactoryConfigurator> configureBus = default)
        {
            var topologyConfiguration = new ServiceBusTopologyConfiguration(AzureBusFactory.MessageTopology);
            var busConfiguration = new ServiceBusBusConfiguration(topologyConfiguration);

            ConfigureApplicationInsights(services);

            services.AddSingleton<IServiceBusBusConfiguration>(busConfiguration)
                .AddSingleton<IMessageReceiver, MessageReceiver>()
                .AddSingleton<IAsyncBusHandle, AsyncBusHandle>()
                .AddMassTransit(cfg =>
                {
                    configure?.Invoke(cfg);

                    cfg.AddBus(configureBus);
                });

            return services;
        }

        static void AddBus(this IRegistrationConfigurator<IServiceProvider> configurator, Action<IServiceBusBusFactoryConfigurator> configure = null)
        {
            configurator.AddBus(context =>
            {
                IOptions<ServiceBusOptions> options = context.Container.GetRequiredService<IOptions<ServiceBusOptions>>();

                options.Value.MessageHandlerOptions.AutoComplete = true;

                IServiceBusBusConfiguration busConfiguration = context.Container.GetRequiredService<IServiceBusBusConfiguration>();

                var busFactoryConfigurator = new ServiceBusBusFactoryConfigurator(busConfiguration);

                busFactoryConfigurator.Host(options.Value.ConnectionString);
                busFactoryConfigurator.UseServiceBusMessageScheduler();

                configure?.Invoke(busFactoryConfigurator);

                return busFactoryConfigurator.Build();
            });
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
