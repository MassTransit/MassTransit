namespace MassTransit.AmazonSqsTransport.Contexts
{
    using Configuration;
    using Context;
    using Topology.Builders;


    public class SqsQueueReceiveEndpointContext :
        BaseReceiveEndpointContext,
        SqsReceiveEndpointContext
    {
        readonly IAmazonSqsHostConfiguration _hostConfiguration;

        public SqsQueueReceiveEndpointContext(IAmazonSqsHostConfiguration hostConfiguration, IAmazonSqsReceiveEndpointConfiguration configuration,
            BrokerTopology brokerTopology)
            : base(hostConfiguration, configuration)
        {
            _hostConfiguration = hostConfiguration;
            BrokerTopology = brokerTopology;
        }

        public BrokerTopology BrokerTopology { get; }

        protected override ISendTransportProvider CreateSendTransportProvider()
        {
            return _hostConfiguration.ConnectionContextSupervisor;
        }

        protected override IPublishTransportProvider CreatePublishTransportProvider()
        {
            return _hostConfiguration.ConnectionContextSupervisor;
        }
    }
}
