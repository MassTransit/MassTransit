namespace MassTransit.Middleware.InMemoryOutbox
{
    using System.Threading.Tasks;


    public class InMemoryOutboxPublishEndpointProvider :
        IPublishEndpointProvider
    {
        readonly OutboxContext _outboxContext;
        readonly IPublishEndpointProvider _publishEndpointProvider;

        public InMemoryOutboxPublishEndpointProvider(OutboxContext outboxContext, IPublishEndpointProvider publishEndpointProvider)
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
