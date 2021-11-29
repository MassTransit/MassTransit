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

        RabbitMqEndpointConfiguration(IEndpointConfiguration parentConfiguration, IRabbitMqTopologyConfiguration topologyConfiguration)
            : base(parentConfiguration, topologyConfiguration)
        {
            Topology = topologyConfiguration;
        }

        public new IRabbitMqTopologyConfiguration Topology { get; }

        public IRabbitMqEndpointConfiguration CreateEndpointConfiguration()
        {
            var topologyConfiguration = new RabbitMqTopologyConfiguration(Topology);

            return new RabbitMqEndpointConfiguration(this, topologyConfiguration);
        }
    }
}
