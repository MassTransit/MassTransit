namespace MassTransit.Azure.ServiceBus.Core.Contexts
{
    using Configuration;
    using Context;
    using Topology;
    using Transport;


    public class ServiceBusEntityReceiveEndpointContext :
        BaseReceiveEndpointContext,
        ServiceBusReceiveEndpointContext
    {
        readonly IServiceBusHostControl _host;

        public ServiceBusEntityReceiveEndpointContext(IServiceBusHostControl host, IServiceBusEntityEndpointConfiguration configuration,
            BrokerTopology brokerTopology)
            : base(configuration)
        {
            _host = host;

            BrokerTopology = brokerTopology;
        }

        public BrokerTopology BrokerTopology { get; }

        protected override ISendTransportProvider CreateSendTransportProvider()
        {
            return new SendEndpointSendTransportProvider(_host);
        }

        protected override IPublishTransportProvider CreatePublishTransportProvider()
        {
            return new PublishTransportProvider(_host);
        }
    }
}
