namespace MassTransit.RabbitMqTransport.Configuration
{
    using MassTransit.Configuration;


    public class RabbitMqEndpointConfiguration :
        EndpointConfiguration,
        IRabbitMqEndpointConfiguration
    {
        public RabbitMqEndpointConfiguration(IRabbitMqTopologyConfiguration topologyConfiguration)
            : base(topologyConfiguration)
        {
            Topology = topologyConfiguration;
        }

        RabbitMqEndpointConfiguration(IEndpointConfiguration parentConfiguration, IRabbitMqTopologyConfiguration topologyConfiguration, bool isBusEndpoint)
            : base(parentConfiguration, topologyConfiguration, isBusEndpoint)
        {
            Topology = topologyConfiguration;
        }

        public new IRabbitMqTopologyConfiguration Topology { get; }

        public IRabbitMqEndpointConfiguration CreateEndpointConfiguration(bool isBusEndpoint)
        {
            var topologyConfiguration = new RabbitMqTopologyConfiguration(Topology);

            return new RabbitMqEndpointConfiguration(this, topologyConfiguration, isBusEndpoint);
        }
    }
}
