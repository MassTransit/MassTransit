using System.Threading.Tasks;

namespace MassTransit.Transactions
{
    public class TransactionalBusPublishEndpointProvider :
        IPublishEndpointProvider
    {
        private readonly BaseTransactionalBus _bus;
        private readonly IPublishEndpointProvider _publishEndpointProvider;

        public TransactionalBusPublishEndpointProvider(BaseTransactionalBus bus, IPublishEndpointProvider publishEndpointProvider)
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

            return new TransactionalBusSendEndpoint(_bus, endpoint);
        }
    }
}
