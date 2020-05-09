namespace MassTransit
{
    using System;


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
        /// Add a RabbitMQ bus
        /// </summary>
        /// <param name="configurator">The registration configurator</param>
        /// <param name="configure">The configure callback method</param>
        /// <typeparam name="TContainerContext"></typeparam>
        public static void AddInMemoryBus<TContainerContext>(this IRegistrationConfigurator<TContainerContext> configurator,
            Action<IRegistrationContext<TContainerContext>, IInMemoryBusFactoryConfigurator> configure)
            where TContainerContext : class
        {
            IBusControl BusFactory(IRegistrationContext<TContainerContext> context)
            {
                return InMemoryBus.Create(cfg =>
                {
                    configure(context, cfg);
                });
            }

            configurator.AddBus(BusFactory);
        }
    }
}
