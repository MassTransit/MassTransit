namespace MassTransit.AmazonSqsTransport.Configuration
{
    using MassTransit.Configuration;


    public class AmazonSqsEndpointConfiguration :
        EndpointConfiguration,
        IAmazonSqsEndpointConfiguration
    {
        readonly IAmazonSqsTopologyConfiguration _topologyConfiguration;

        public AmazonSqsEndpointConfiguration(IAmazonSqsTopologyConfiguration topologyConfiguration)
            : base(topologyConfiguration)
        {
            _topologyConfiguration = topologyConfiguration;
        }

        AmazonSqsEndpointConfiguration(IEndpointConfiguration parentConfiguration, IAmazonSqsTopologyConfiguration topologyConfiguration)
            : base(parentConfiguration, topologyConfiguration)
        {
            _topologyConfiguration = topologyConfiguration;
        }

        public new IAmazonSqsTopologyConfiguration Topology => _topologyConfiguration;

        public IAmazonSqsEndpointConfiguration CreateEndpointConfiguration()
        {
            var topologyConfiguration = new AmazonSqsTopologyConfiguration(_topologyConfiguration);

            return new AmazonSqsEndpointConfiguration(this, topologyConfiguration);
        }
    }
}
