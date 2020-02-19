namespace MassTransit
{
    using System;
    using Context;
    using Transports;
    using Transports.InMemory.Configuration;


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
            Action<TContainerContext, IInMemoryBusFactoryConfigurator> configure)
        {
            IBusControl BusFactory(TContainerContext context)
            {
                return InMemoryBus.Create(cfg =>
                {
                    configure(context, cfg);
                });
            }

            configurator.AddBus(BusFactory);
        }

        /// <summary>
        /// Create a mediator, which sends messages to consumers, handlers, and sagas. Messages are dispatched to the consumers asynchronously.
        /// Consumers are not directly coupled to the sender. Can be used entirely in-memory without a broker.
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IMediator CreateInMemoryMediator(this IBusFactorySelector selector, Action<IInMemoryReceiveEndpointConfigurator> configure)
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            var topologyConfiguration = new InMemoryTopologyConfiguration(InMemoryBus.MessageTopology);
            var busConfiguration = new InMemoryBusConfiguration(topologyConfiguration, new Uri("loopback://localhost"));

            var receiveEndpointConfiguration = busConfiguration.HostConfiguration.CreateReceiveEndpointConfiguration("unspecified");

            var configurator = new InMemoryReceivePipeDispatcherConfiguration(receiveEndpointConfiguration);

            configure(configurator);

            var receiver = configurator.Build();

            return new InMemoryMediator(LogContext.Current, receiveEndpointConfiguration, receiver);
        }
    }
}
