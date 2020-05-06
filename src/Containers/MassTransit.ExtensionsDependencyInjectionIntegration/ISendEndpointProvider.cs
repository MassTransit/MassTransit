namespace MassTransit.ExtensionsDependencyInjectionIntegration
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;


    public interface ISendEndpointProvider<in TBus> :
        ISendEndpointProvider
        where TBus : class
    {
    }


    class SendEndpointProvider<TBus> : ISendEndpointProvider<TBus>
        where TBus : class
    {
        readonly ISendEndpointProvider _sendEndpointProvider;
        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _sendEndpointProvider.ConnectSendObserver(observer);
        }

        public Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            return _sendEndpointProvider.GetSendEndpoint(address);
        }

        public SendEndpointProvider(ISendEndpointProvider sendEndpointProvider)
        {
            _sendEndpointProvider = sendEndpointProvider;
        }
    }
}
