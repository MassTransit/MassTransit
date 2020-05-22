namespace MassTransit.ActiveMqTransport.Contexts
{
    using Configuration;
    using Context;
    using Topology.Builders;


    public class ActiveMqConsumerReceiveEndpointContext :
        BaseReceiveEndpointContext,
        ActiveMqReceiveEndpointContext
    {
        readonly IActiveMqHostConfiguration _hostConfiguration;

        public ActiveMqConsumerReceiveEndpointContext(IActiveMqHostConfiguration hostConfiguration, IActiveMqReceiveEndpointConfiguration configuration,
            BrokerTopology brokerTopology)
            : base(configuration)
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
