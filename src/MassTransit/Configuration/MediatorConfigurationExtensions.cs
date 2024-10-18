namespace MassTransit
{
    using System;
    using Configuration;
    using InMemoryTransport.Configuration;
    using Mediator;


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
        public static IMediator CreateMediator(this IBusFactorySelector selector, Action<IMediatorConfigurator> configure)
        {
            return CreateMediator(selector, null, configure);
        }

        /// <summary>
        /// Create a mediator, which sends messages to consumers, handlers, and sagas. Messages are dispatched to the consumers asynchronously.
        /// Consumers are not directly coupled to the sender. Can be used entirely in-memory without a broker.
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="configure"></param>
        /// <param name="baseAddress"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IMediator CreateMediator(this IBusFactorySelector selector, Uri baseAddress, Action<IMediatorConfigurator> configure)
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            baseAddress ??= new Uri("loopback://localhost/");
            var topologyConfiguration = new InMemoryTopologyConfiguration(InMemoryBus.CreateMessageTopology());
            var busConfiguration = new InMemoryBusConfiguration(topologyConfiguration, baseAddress);

            if (LogContext.Current != null)
                busConfiguration.HostConfiguration.LogContext = LogContext.Current;

            var endpointConfiguration = busConfiguration.HostConfiguration.CreateReceiveEndpointConfiguration("mediator");

            var configurator = new MediatorConfiguration(busConfiguration.HostConfiguration, endpointConfiguration);

            configure(configurator);

            var mediatorDispatcher = configurator.Build();

            var responseEndpointConfiguration = busConfiguration.HostConfiguration.CreateReceiveEndpointConfiguration("response");
            var responseConfigurator = new ReceivePipeDispatcherConfiguration(busConfiguration.HostConfiguration, responseEndpointConfiguration);

            configurator = new MediatorConfiguration(busConfiguration.HostConfiguration, responseEndpointConfiguration);

            configure(configurator);

            var responseDispatcher = responseConfigurator.Build();

            return new MassTransitMediator(LogContext.Current, endpointConfiguration, mediatorDispatcher, responseEndpointConfiguration, responseDispatcher);
        }
    }
}
