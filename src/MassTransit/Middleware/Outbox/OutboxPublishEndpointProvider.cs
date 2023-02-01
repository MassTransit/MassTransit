namespace MassTransit.Middleware.Outbox
{
    using System.Threading.Tasks;


    public class OutboxPublishEndpointProvider :
        IPublishEndpointProvider
    {
        readonly OutboxSendContext _context;
        readonly IPublishEndpointProvider _publishEndpointProvider;

        public OutboxPublishEndpointProvider(OutboxSendContext context, IPublishEndpointProvider publishEndpointProvider)
        {
            _context = context;
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

            return new OutboxSendEndpoint(_context, endpoint);
        }
    }
}
