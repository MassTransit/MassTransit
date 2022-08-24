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

        InMemoryEndpointConfiguration(IInMemoryEndpointConfiguration parentConfiguration, IInMemoryTopologyConfiguration topologyConfiguration,
            bool isBusEndpoint)
            : base(parentConfiguration, topologyConfiguration, isBusEndpoint)
        {
            _topologyConfiguration = topologyConfiguration;
        }

        IInMemoryTopologyConfiguration IInMemoryEndpointConfiguration.Topology => _topologyConfiguration;

        public IInMemoryEndpointConfiguration CreateEndpointConfiguration(bool isBusEndpoint)
        {
            var topologyConfiguration = new InMemoryTopologyConfiguration(_topologyConfiguration);

            return new InMemoryEndpointConfiguration(this, topologyConfiguration, isBusEndpoint);
        }
    }
}
