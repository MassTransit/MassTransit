namespace MassTransit.RabbitMqTransport.Contexts
{
    using Configuration;
    using Context;
    using Topology.Builders;
    using Transport;


    public class RabbitMqQueueReceiveEndpointContext :
        BaseReceiveEndpointContext,
        RabbitMqReceiveEndpointContext
    {
        readonly IRabbitMqHostControl _host;

        public RabbitMqQueueReceiveEndpointContext(IRabbitMqHostControl host, IRabbitMqReceiveEndpointConfiguration configuration, BrokerTopology brokerTopology)
            : base(configuration)
        {
            _host = host;

            ExclusiveConsumer = configuration.Settings.ExclusiveConsumer;
            BrokerTopology = brokerTopology;
        }

        public BrokerTopology BrokerTopology { get; }

        public bool ExclusiveConsumer { get; }

        protected override ISendTransportProvider CreateSendTransportProvider()
        {
            return new SendTransportProvider(_host);
        }

        protected override IPublishTransportProvider CreatePublishTransportProvider()
        {
            return new PublishTransportProvider(_host);
        }
    }
}
