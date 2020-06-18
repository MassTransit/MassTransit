namespace MassTransit.Transactions
{
    using System.Threading.Tasks;
    using GreenPipes;


    public class TransactionalPublishEndpointProvider :
        IPublishEndpointProvider
    {
        readonly TransactionalBus _bus;
        readonly IPublishEndpointProvider _publishEndpointProvider;

        public TransactionalPublishEndpointProvider(TransactionalBus bus, IPublishEndpointProvider publishEndpointProvider)
        {
            _bus = bus;
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

            return new TransactionalSendEndpoint(_bus, endpoint);
        }
    }
}
