namespace MassTransit.Middleware.Outbox
{
    using System.Threading.Tasks;


    public class OutboxPublishEndpointProvider :
        IPublishEndpointProvider
    {
        readonly OutboxSendContext _outboxContext;
        readonly IPublishEndpointProvider _publishEndpointProvider;

        public OutboxPublishEndpointProvider(OutboxSendContext outboxContext, IPublishEndpointProvider publishEndpointProvider)
        {
            _outboxContext = outboxContext;
            _publishEndpointProvider = publishEndpointProvider;
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _publishEndpointProvider.ConnectPublishObserver(observer);
        }

        public async Task<ISendEndpoint> GetPublishSendEndpoint<T>()
            where T : class
        {
            var endpoint = await _publishEndpointProvider.GetPublishSendEndpoint<T>().ConfigureAwait(false);

            return new OutboxSendEndpoint(_outboxContext, endpoint);
        }
    }
}
