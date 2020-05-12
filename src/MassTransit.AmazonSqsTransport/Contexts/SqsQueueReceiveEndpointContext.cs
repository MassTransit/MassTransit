namespace MassTransit.AmazonSqsTransport.Contexts
{
    using Configuration;
    using Context;
    using Topology.Builders;
    using Transport;


    public class SqsQueueReceiveEndpointContext :
        BaseReceiveEndpointContext,
        SqsReceiveEndpointContext
    {
        readonly IAmazonSqsHostControl _host;

        public SqsQueueReceiveEndpointContext(IAmazonSqsHostControl host, IAmazonSqsReceiveEndpointConfiguration configuration, BrokerTopology brokerTopology)
            : base(configuration)
        {
            _host = host;
            BrokerTopology = brokerTopology;
        }

        public BrokerTopology BrokerTopology { get; }

        protected override ISendTransportProvider CreateSendTransportProvider()
        {
            return new AmazonSqsSendTransportProvider(_host);
        }

        protected override IPublishTransportProvider CreatePublishTransportProvider()
        {
            return new AmazonSqsPublishTransportProvider(_host);
        }
    }
}
