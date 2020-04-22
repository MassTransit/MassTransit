namespace MassTransit.Pipeline.PayloadInjector
{
    using System.Threading.Tasks;
    using GreenPipes;


    public class PayloadPublishEndpointProvider<TPayload> :
        IPublishEndpointProvider
        where TPayload : class
    {
        readonly IPublishEndpointProvider _provider;
        readonly PayloadFactory<TPayload> _payloadFactory;

        public PayloadPublishEndpointProvider(IPublishEndpointProvider provider, PayloadFactory<TPayload> payloadFactory)
        {
            _provider = provider;
            _payloadFactory = payloadFactory;
        }

        ConnectHandle IPublishObserverConnector.ConnectPublishObserver(IPublishObserver observer)
        {
            return _provider.ConnectPublishObserver(observer);
        }

        async Task<ISendEndpoint> IPublishEndpointProvider.GetPublishSendEndpoint<T>()
            where T : class
        {
            var endpoint = await _provider.GetPublishSendEndpoint<T>().ConfigureAwait(false);

            var payload = _payloadFactory();

            return new PayloadSendEndpoint<TPayload>(endpoint, payload);
        }
    }
}
