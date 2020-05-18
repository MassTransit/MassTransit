namespace MassTransit.RabbitMqTransport.Configuration
{
    using MassTransit.Configuration;


    public class RabbitMqEndpointConfiguration :
        EndpointConfiguration,
        IRabbitMqEndpointConfiguration
    {
        readonly IRabbitMqTopologyConfiguration _topologyConfiguration;

        public RabbitMqEndpointConfiguration(IRabbitMqTopologyConfiguration topologyConfiguration)
            : base(topologyConfiguration)
        {
            _topologyConfiguration = topologyConfiguration;
        }

        RabbitMqEndpointConfiguration(IEndpointConfiguration parentConfiguration, IRabbitMqTopologyConfiguration topologyConfiguration)
            : base(parentConfiguration, topologyConfiguration)
        {
            _topologyConfiguration = topologyConfiguration;
        }

        public new IRabbitMqTopologyConfiguration Topology => _topologyConfiguration;

        public IRabbitMqEndpointConfiguration CreateEndpointConfiguration()
        {
            var topologyConfiguration = new RabbitMqTopologyConfiguration(_topologyConfiguration);

            return new RabbitMqEndpointConfiguration(this, topologyConfiguration);
        }
    }
}
