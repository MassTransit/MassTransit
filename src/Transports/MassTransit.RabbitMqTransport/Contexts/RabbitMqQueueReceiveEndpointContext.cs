namespace MassTransit.RabbitMqTransport.Contexts
{
    using Configuration;
    using Context;
    using Integration;
    using Topology.Builders;


    public class RabbitMqQueueReceiveEndpointContext :
        BaseReceiveEndpointContext,
        RabbitMqReceiveEndpointContext
    {
        readonly IRabbitMqHostConfiguration _hostConfiguration;

        public RabbitMqQueueReceiveEndpointContext(IRabbitMqHostConfiguration hostConfiguration, IRabbitMqReceiveEndpointConfiguration configuration,
            BrokerTopology brokerTopology)
            : base(hostConfiguration, configuration)
        {
            _hostConfiguration = hostConfiguration;

            ExclusiveConsumer = configuration.Settings.ExclusiveConsumer;
            BrokerTopology = brokerTopology;

            ModelContextSupervisor = hostConfiguration.CreateModelContextSupervisor();
        }

        public BrokerTopology BrokerTopology { get; }

        public bool ExclusiveConsumer { get; }

        public IModelContextSupervisor ModelContextSupervisor { get; }

        protected override ISendTransportProvider CreateSendTransportProvider()
        {
            return _hostConfiguration.CreateSendTransportProvider(ModelContextSupervisor);
        }

        protected override IPublishTransportProvider CreatePublishTransportProvider()
        {
            return _hostConfiguration.CreatePublishTransportProvider(ModelContextSupervisor);
        }
    }
}
