namespace MassTransit
{
    using Microsoft.Azure.WebJobs.ServiceBus;
    using Microsoft.Extensions.DependencyInjection;
    using WebJobs.EventHubsIntegration;


    public static class AzureFunctionsEventHubConfigurationExtensions
    {
        /// <summary>
        /// Add Event Hub for MassTransit on Azure Functions, which sits on top of Azure Service Bus, and configures
        /// <see cref="IEventReceiver"/> for use by functions to handle messages. Uses <see cref="ServiceBusOptions.ConnectionString"/>
        /// to connect to Azure Service Bus.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMassTransitEventHub(this IServiceCollection services)
        {
            services.AddSingleton<IEventReceiver, EventReceiver>();

            return services;
        }
    }
}
