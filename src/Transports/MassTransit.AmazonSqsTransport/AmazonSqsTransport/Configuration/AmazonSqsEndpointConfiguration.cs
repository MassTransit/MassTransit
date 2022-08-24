namespace MassTransit.AmazonSqsTransport.Configuration
{
    using MassTransit.Configuration;


    public class AmazonSqsEndpointConfiguration :
        EndpointConfiguration,
        IAmazonSqsEndpointConfiguration
    {
        public AmazonSqsEndpointConfiguration(IAmazonSqsTopologyConfiguration topologyConfiguration)
            : base(topologyConfiguration)
        {
            Topology = topologyConfiguration;
        }

        AmazonSqsEndpointConfiguration(IEndpointConfiguration parentConfiguration, IAmazonSqsTopologyConfiguration topologyConfiguration, bool isBusEndpoint)
            : base(parentConfiguration, topologyConfiguration, isBusEndpoint)
        {
            Topology = topologyConfiguration;
        }

        public new IAmazonSqsTopologyConfiguration Topology { get; }

        public IAmazonSqsEndpointConfiguration CreateEndpointConfiguration(bool isBusEndpoint)
        {
            var topologyConfiguration = new AmazonSqsTopologyConfiguration(Topology);

            return new AmazonSqsEndpointConfiguration(this, topologyConfiguration, isBusEndpoint);
        }
    }
}
