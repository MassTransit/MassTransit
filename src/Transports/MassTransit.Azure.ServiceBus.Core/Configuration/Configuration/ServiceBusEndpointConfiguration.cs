namespace MassTransit.Azure.ServiceBus.Core.Configuration
{
    using MassTransit.Configuration;


    public class ServiceBusEndpointConfiguration :
        EndpointConfiguration,
        IServiceBusEndpointConfiguration
    {
        public ServiceBusEndpointConfiguration(IServiceBusTopologyConfiguration topologyConfiguration)
            : base(topologyConfiguration)
        {
            Topology = topologyConfiguration;
        }

        ServiceBusEndpointConfiguration(IServiceBusEndpointConfiguration parentConfiguration, IServiceBusTopologyConfiguration topologyConfiguration)
            : base(parentConfiguration, topologyConfiguration)
        {
            Topology = topologyConfiguration;
        }

        public new IServiceBusTopologyConfiguration Topology { get; }

        public IServiceBusEndpointConfiguration CreateEndpointConfiguration()
        {
            var topologyConfiguration = new ServiceBusTopologyConfiguration(Topology);

            return new ServiceBusEndpointConfiguration(this, topologyConfiguration);
        }
    }
}
