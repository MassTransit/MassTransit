namespace MassTransit.Transports.InMemory.Configuration
{
    using System;
    using GreenPipes.Caching;
    using MassTransit.Configuration;


    public interface IInMemoryHostConfiguration :
        IHostConfiguration,
        IReceiveConfigurator<IInMemoryReceiveEndpointConfigurator>
    {
        /// <summary>
        /// Set the host's base address
        /// </summary>
        Uri BaseAddress { set; }

        /// <summary>
        /// Sets the maximum number of threads used by an in-memory transport, for partitioning
        /// the input queue. This setting also specifies how many threads will be used for dispatching
        /// messages to consumers.
        /// </summary>
        int TransportConcurrencyLimit { get; set; }

        IInMemoryHostConfigurator Configurator { get; }

        CacheSettings SendTransportCacheSettings { get; }

        IInMemoryHost Proxy { get; }

        void ApplyEndpointDefinition(IInMemoryReceiveEndpointConfigurator configurator, IEndpointDefinition definition);

        IInMemoryReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName,
            Action<IInMemoryReceiveEndpointConfigurator> configure = null);

        IInMemoryReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName, IInMemoryEndpointConfiguration endpointConfiguration,
            Action<IInMemoryReceiveEndpointConfigurator> configure = null);
    }
}
