namespace MassTransit
{
    using System;
    using Configuration;
    using Context;
    using Mediator;
    using Transports.InMemory.Configuration;


    public static class MediatorConfigurationExtensions
    {
        /// <summary>
        /// Create a mediator, which sends messages to consumers, handlers, and sagas. Messages are dispatched to the consumers asynchronously.
        /// Consumers are not directly coupled to the sender. Can be used entirely in-memory without a broker.
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IMediator CreateMediator(this IBusFactorySelector selector, Action<IReceiveEndpointConfigurator> configure)
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            var topologyConfiguration = new InMemoryTopologyConfiguration(InMemoryBus.MessageTopology);
            var busConfiguration = new InMemoryBusConfiguration(topologyConfiguration, new Uri("loopback://localhost"));

            var endpointConfiguration = busConfiguration.HostConfiguration.CreateReceiveEndpointConfiguration("mediator");

            var configurator = new ReceivePipeDispatcherConfiguration(endpointConfiguration);

            configure(configurator);

            var mediatorDispatcher = configurator.Build();

            var responseEndpointConfiguration = busConfiguration.HostConfiguration.CreateReceiveEndpointConfiguration("response");
            var responseConfigurator = new ReceivePipeDispatcherConfiguration(responseEndpointConfiguration);

            var responseDispatcher = responseConfigurator.Build();

            return new MassTransitMediator(LogContext.Current, endpointConfiguration, mediatorDispatcher, responseEndpointConfiguration, responseDispatcher);
        }
    }
}
