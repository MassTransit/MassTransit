namespace MassTransit.Transports.InMemory.Contexts
{
    using Configuration;
    using Context;


    public class InMemoryReceiveEndpointContext :
        BaseReceiveEndpointContext
    {
        readonly IInMemoryHostConfiguration _hostConfiguration;

        public InMemoryReceiveEndpointContext(IInMemoryHostConfiguration hostConfiguration, IInMemoryReceiveEndpointConfiguration configuration)
            : base(configuration)
        {
            _hostConfiguration = hostConfiguration;
        }

        protected override ISendTransportProvider CreateSendTransportProvider()
        {
            return _hostConfiguration.TransportProvider;
        }

        protected override IPublishTransportProvider CreatePublishTransportProvider()
        {
            return _hostConfiguration.TransportProvider;
        }
    }
}
