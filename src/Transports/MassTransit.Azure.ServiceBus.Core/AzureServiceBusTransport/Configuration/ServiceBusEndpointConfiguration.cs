namespace MassTransit.AzureServiceBusTransport.Configuration
{
    using MassTransit.Configuration;


    public class ServiceBusEndpointConfiguration :
        EndpointConfiguration,
        IServiceBusEndpointConfiguration
    {
        protected ServiceBusEndpointConfiguration(IServiceBusTopologyConfiguration topologyConfiguration)
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
