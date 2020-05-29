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

        ActiveMqEndpointConfiguration(IEndpointConfiguration parentConfiguration, IActiveMqTopologyConfiguration topologyConfiguration)
            : base(parentConfiguration, topologyConfiguration)
        {
            Topology = topologyConfiguration;
        }

        public new IActiveMqTopologyConfiguration Topology { get; }

        public IActiveMqEndpointConfiguration CreateEndpointConfiguration()
        {
            var topologyConfiguration = new ActiveMqTopologyConfiguration(Topology);

            return new ActiveMqEndpointConfiguration(this, topologyConfiguration);
        }
    }
}
