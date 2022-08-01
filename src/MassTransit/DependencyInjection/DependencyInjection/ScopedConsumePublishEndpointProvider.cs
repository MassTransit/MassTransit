namespace MassTransit.DependencyInjection
{
    using System.Threading.Tasks;


    public class ScopedConsumePublishEndpointProvider<TScope> :
        IPublishEndpointProvider
        where TScope : class
    {
        readonly ConsumeContext _consumeContext;
        readonly IPublishEndpointProvider _provider;
        readonly TScope _scope;

        public ScopedConsumePublishEndpointProvider(IPublishEndpointProvider provider, ConsumeContext consumeContext, TScope scope)
        {
            _provider = provider;
            _consumeContext = consumeContext;
            _scope = scope;
        }

        ConnectHandle IPublishObserverConnector.ConnectPublishObserver(IPublishObserver observer)
        {
            return _provider.ConnectPublishObserver(observer);
        }

        async Task<ISendEndpoint> IPublishEndpointProvider.GetPublishSendEndpoint<T>()
            where T : class
        {
            var endpoint = await _provider.GetPublishEndpoint<T>(_consumeContext, default).ConfigureAwait(false);

            return new ScopedSendEndpoint<TScope>(endpoint, _scope);
        }
    }
}
