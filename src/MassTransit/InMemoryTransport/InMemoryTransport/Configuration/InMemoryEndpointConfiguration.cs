namespace MassTransit.InMemoryTransport.Configuration
{
    using MassTransit.Configuration;


    public class InMemoryEndpointConfiguration :
        EndpointConfiguration,
        IInMemoryEndpointConfiguration
    {
        readonly IInMemoryTopologyConfiguration _topologyConfiguration;

        protected InMemoryEndpointConfiguration(IInMemoryTopologyConfiguration topologyConfiguration)
            : base(topologyConfiguration)
        {
            _topologyConfiguration = topologyConfiguration;
        }

        InMemoryEndpointConfiguration(IInMemoryEndpointConfiguration parentConfiguration, IInMemoryTopologyConfiguration topologyConfiguration)
            : base(parentConfiguration, topologyConfiguration)
        {
            _topologyConfiguration = topologyConfiguration;
        }

        IInMemoryTopologyConfiguration IInMemoryEndpointConfiguration.Topology => _topologyConfiguration;

        public IInMemoryEndpointConfiguration CreateEndpointConfiguration()
        {
            var topologyConfiguration = new InMemoryTopologyConfiguration(_topologyConfiguration);

            return new InMemoryEndpointConfiguration(this, topologyConfiguration);
        }
    }
}
