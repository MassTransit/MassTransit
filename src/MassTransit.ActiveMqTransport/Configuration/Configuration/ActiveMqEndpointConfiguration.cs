namespace MassTransit.ActiveMqTransport.Configuration
{
    using MassTransit.Configuration;


    public class ActiveMqEndpointConfiguration :
        EndpointConfiguration,
        IActiveMqEndpointConfiguration
    {
        readonly IActiveMqTopologyConfiguration _topologyConfiguration;

        protected ActiveMqEndpointConfiguration(IActiveMqTopologyConfiguration topologyConfiguration)
            : base(topologyConfiguration)
        {
            _topologyConfiguration = topologyConfiguration;
        }

        ActiveMqEndpointConfiguration(IEndpointConfiguration parentConfiguration, IActiveMqTopologyConfiguration topologyConfiguration)
            : base(parentConfiguration, topologyConfiguration)
        {
            _topologyConfiguration = topologyConfiguration;
        }

        public new IActiveMqTopologyConfiguration Topology => _topologyConfiguration;

        public IActiveMqEndpointConfiguration CreateEndpointConfiguration()
        {
            var topologyConfiguration = new ActiveMqTopologyConfiguration(_topologyConfiguration);

            return new ActiveMqEndpointConfiguration(this, topologyConfiguration);
        }
    }
}
