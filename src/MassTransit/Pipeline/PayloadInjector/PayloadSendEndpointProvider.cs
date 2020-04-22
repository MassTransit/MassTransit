namespace MassTransit.Pipeline.PayloadInjector
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;


    public class PayloadSendEndpointProvider<TPayload> :
        ISendEndpointProvider
        where TPayload : class
    {
        readonly ISendEndpointProvider _provider;
        readonly PayloadFactory<TPayload> _payloadFactory;

        public PayloadSendEndpointProvider(ISendEndpointProvider provider, PayloadFactory<TPayload> payloadFactory)
        {
            _provider = provider;
            _payloadFactory = payloadFactory;
        }

        ConnectHandle ISendObserverConnector.ConnectSendObserver(ISendObserver observer)
        {
            return _provider.ConnectSendObserver(observer);
        }

        async Task<ISendEndpoint> ISendEndpointProvider.GetSendEndpoint(Uri address)
        {
            var endpoint = await _provider.GetSendEndpoint(address).ConfigureAwait(false);

            var payload = _payloadFactory();

            return new PayloadSendEndpoint<TPayload>(endpoint, payload);
        }
    }
}
