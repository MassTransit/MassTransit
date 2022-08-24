namespace MassTransit.ActiveMqTransport.Configuration
{
    using MassTransit.Configuration;


    public class ActiveMqEndpointConfiguration :
        EndpointConfiguration,
        IActiveMqEndpointConfiguration
    {
        protected ActiveMqEndpointConfiguration(IActiveMqTopologyConfiguration topologyConfiguration)
            : base(topologyConfiguration)
        {
            Topology = topologyConfiguration;
        }

        ActiveMqEndpointConfiguration(IEndpointConfiguration parentConfiguration, IActiveMqTopologyConfiguration topologyConfiguration, bool isBusEndpoint)
            : base(parentConfiguration, topologyConfiguration, isBusEndpoint)
        {
            Topology = topologyConfiguration;
        }

        public new IActiveMqTopologyConfiguration Topology { get; }

        public IActiveMqEndpointConfiguration CreateEndpointConfiguration(bool isBusEndpoint)
        {
            var topologyConfiguration = new ActiveMqTopologyConfiguration(Topology);

            return new ActiveMqEndpointConfiguration(this, topologyConfiguration, isBusEndpoint);
        }
    }
}
