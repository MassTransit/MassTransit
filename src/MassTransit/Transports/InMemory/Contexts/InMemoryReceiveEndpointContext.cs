namespace MassTransit.Transports.InMemory.Contexts
{
    using Configuration;
    using Context;
    using Topology.Configurators;


    public class InMemoryReceiveEndpointContext :
        BaseReceiveEndpointContext
    {
        readonly IInMemoryPublishTopologyConfigurator _publish;
        readonly ISendTransportProvider _sendTransportProvider;

        public InMemoryReceiveEndpointContext(IInMemoryReceiveEndpointConfiguration configuration, ISendTransportProvider sendTransportProvider)
            : base(configuration)
        {
            _sendTransportProvider = sendTransportProvider;

            _publish = configuration.Topology.Publish;
        }

        protected override ISendTransportProvider CreateSendTransportProvider()
        {
            return _sendTransportProvider;
        }

        protected override IPublishTransportProvider CreatePublishTransportProvider()
        {
            return new InMemoryPublishTransportProvider(_sendTransportProvider, _publish);
        }
    }
}