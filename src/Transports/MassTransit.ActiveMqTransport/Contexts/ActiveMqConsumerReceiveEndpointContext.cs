namespace MassTransit.ActiveMqTransport.Contexts
{
    using Configuration;
    using Context;
    using Topology.Builders;
    using Transport;


    public class ActiveMqConsumerReceiveEndpointContext :
        BaseReceiveEndpointContext,
        ActiveMqReceiveEndpointContext
    {
        readonly IActiveMqHostControl _host;

        public ActiveMqConsumerReceiveEndpointContext(IActiveMqHostControl host, IActiveMqReceiveEndpointConfiguration configuration, BrokerTopology
            brokerTopology)
            : base(configuration)
        {
            _host = host;
            BrokerTopology = brokerTopology;
        }

        public BrokerTopology BrokerTopology { get; }

        protected override ISendTransportProvider CreateSendTransportProvider()
        {
            return new ActiveMqSendTransportProvider(_host);
        }

        protected override IPublishTransportProvider CreatePublishTransportProvider()
        {
            return new ActiveMqPublishTransportProvider(_host);
        }
    }
}
