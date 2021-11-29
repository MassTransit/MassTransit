namespace MassTransit
{
    using System;
    using InMemoryTransport.Configuration;


    public static class InMemoryConfigurationExtensions
    {
        /// <summary>
        /// Configure and create an in-memory bus
        /// </summary>
        /// <param name="selector">Hang off the selector interface for visibility</param>
        /// <param name="configure">The configuration callback to configure the bus</param>
        /// <returns></returns>
        public static IBusControl CreateUsingInMemory(this IBusFactorySelector selector, Action<IInMemoryBusFactoryConfigurator> configure)
        {
            return InMemoryBus.Create(configure);
        }

        /// <summary>
        /// Configure and create an in-memory bus
        /// </summary>
        /// <param name="selector">Hang off the selector interface for visibility</param>
        /// <param name="baseAddress">Override the default base address</param>
        /// <param name="configure">The configuration callback to configure the bus</param>
        /// <returns></returns>
        public static IBusControl CreateUsingInMemory(this IBusFactorySelector selector, Uri baseAddress, Action<IInMemoryBusFactoryConfigurator> configure)
        {
            return InMemoryBus.Create(baseAddress, configure);
        }

        /// <summary>
        /// Configure MassTransit to use the In-Memory transport.
        /// </summary>
        /// <param name="configurator">The registration configurator (configured via AddMassTransit)</param>
        /// <param name="configure">The configuration callback for the bus factory</param>
        public static void UsingInMemory(this IBusRegistrationConfigurator configurator,
            Action<IBusRegistrationContext, IInMemoryBusFactoryConfigurator> configure = null)
        {
            UsingInMemory(configurator, null, configure);
        }

        /// <summary>
        /// Configure MassTransit to use the In-Memory transport.
        /// </summary>
        /// <param name="configurator">The registration configurator (configured via AddMassTransit)</param>
        /// <param name="baseAddress">The base Address of the transport</param>
        /// <param name="configure">The configuration callback for the bus factory</param>
        public static void UsingInMemory(this IBusRegistrationConfigurator configurator, Uri baseAddress,
            Action<IBusRegistrationContext, IInMemoryBusFactoryConfigurator> configure = null)
        {
            configurator.SetBusFactory(new InMemoryRegistrationBusFactory(baseAddress, configure));
        }
    }
}
