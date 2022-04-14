namespace MassTransit.Middleware.Outbox
{
    using System;
    using System.Threading.Tasks;


    public class OutboxSendEndpointProvider :
        ISendEndpointProvider
    {
        readonly OutboxSendContext _outboxContext;
        readonly ISendEndpointProvider _sendEndpointProvider;

        public OutboxSendEndpointProvider(OutboxSendContext outboxContext, ISendEndpointProvider sendEndpointProvider)
        {
            _outboxContext = outboxContext;
            _sendEndpointProvider = sendEndpointProvider;
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _sendEndpointProvider.ConnectSendObserver(observer);
        }

        public async Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(address).ConfigureAwait(false);

            return new OutboxSendEndpoint(_outboxContext, endpoint);
        }
    }
}
