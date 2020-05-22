namespace MassTransit.Azure.ServiceBus.Core
{
    using System;
    using Configuration;


    public static class ServiceBusConfigurationExtensions
    {
        /// <summary>
        /// Configure and create a bus for Azure Service Bus
        /// </summary>
        /// <param name="selector">Hang off the selector interface for visibility</param>
        /// <param name="configure">The configuration callback to configure the bus</param>
        /// <returns></returns>
        public static IBusControl CreateUsingAzureServiceBus(this IBusFactorySelector selector, Action<IServiceBusBusFactoryConfigurator> configure)
        {
            return AzureBusFactory.CreateUsingServiceBus(configure);
        }

        /// <summary>
        /// Configure MassTransit to use Azure Service Bus for the transport.
        /// </summary>
        /// <param name="configurator">The registration configurator (configured via AddMassTransit)</param>
        /// <param name="configure">The configuration callback for the bus factory</param>
        /// <typeparam name="T">The container context type</typeparam>
        public static void UsingAzureServiceBus<T>(this IRegistrationConfigurator<T> configurator,
            Action<IRegistrationContext<T>, IServiceBusBusFactoryConfigurator> configure = null)
            where T : class
        {
            configurator.SetBusFactory(new ServiceBusRegistrationBusFactory<T>(configure));
        }
    }
}
