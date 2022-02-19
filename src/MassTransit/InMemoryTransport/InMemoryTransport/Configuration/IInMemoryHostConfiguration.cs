namespace MassTransit.InMemoryTransport.Configuration
{
    using System;
    using MassTransit.Configuration;


    public interface IInMemoryHostConfiguration :
        IHostConfiguration,
        IReceiveConfigurator<IInMemoryReceiveEndpointConfigurator>
    {
        /// <summary>
        /// Set the host's base address
        /// </summary>
        Uri BaseAddress { set; }

        IInMemoryHostConfigurator Configurator { get; }

        IInMemoryTransportProvider TransportProvider { get; }

        new IInMemoryBusTopology Topology { get; }

        void ApplyEndpointDefinition(IInMemoryReceiveEndpointConfigurator configurator, IEndpointDefinition definition);

        IInMemoryReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName,
            Action<IInMemoryReceiveEndpointConfigurator> configure = null);

        IInMemoryReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName, IInMemoryEndpointConfiguration endpointConfiguration,
            Action<IInMemoryReceiveEndpointConfigurator> configure = null);
    }
}
