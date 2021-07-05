using System;
using MassTransit.ActiveMqTransport.Configurators;

namespace MassTransit.ActiveMqTransport.Configuration
{
    using MassTransit.Configuration;
    using Topology;
    
    public delegate IActiveMqBindingConsumeTopologySpecification ActiveMqBindingConsumeTopologySpecificationFactoryMethod(string topic);

    public interface IActiveMqBusConfiguration :
        IBusConfiguration
    {
        new IActiveMqHostConfiguration HostConfiguration { get; }

        new IActiveMqEndpointConfiguration BusEndpointConfiguration { get; }

        new IActiveMqTopologyConfiguration Topology { get; }

        /// <summary>
        /// Create an endpoint configuration on the bus, which can later be turned into a receive endpoint
        /// </summary>
        /// <returns></returns>
        IActiveMqEndpointConfiguration CreateEndpointConfiguration();

        ActiveMqBindingConsumeTopologySpecificationFactoryMethod BindingConsumeTopologySpecificationFactoryMethod { get; set; }
    }
}
