namespace MassTransit.RabbitMqTransport.Contexts
{
    using Configuration;
    using Context;
    using Integration;
    using Topology.Builders;
    using Transport;


    public class RabbitMqQueueReceiveEndpointContext :
        BaseReceiveEndpointContext,
        RabbitMqReceiveEndpointContext
    {
        readonly IRabbitMqHostControl _host;

        public RabbitMqQueueReceiveEndpointContext(IRabbitMqHostControl host, IModelContextSupervisor supervisor,
            IRabbitMqReceiveEndpointConfiguration configuration,
            BrokerTopology brokerTopology)
            : base(configuration)
        {
            _host = host;
            ModelContextSupervisor = supervisor;

            ExclusiveConsumer = configuration.Settings.ExclusiveConsumer;
            BrokerTopology = brokerTopology;
        }

        public BrokerTopology BrokerTopology { get; }

        public bool ExclusiveConsumer { get; }

        public IModelContextSupervisor ModelContextSupervisor { get; }

        protected override ISendTransportProvider CreateSendTransportProvider()
        {
            return new RabbitMqSendTransportProvider(_host, ModelContextSupervisor);
        }

        protected override IPublishTransportProvider CreatePublishTransportProvider()
        {
            return new RabbitMqPublishTransportProvider(_host, ModelContextSupervisor);
        }
    }
}
