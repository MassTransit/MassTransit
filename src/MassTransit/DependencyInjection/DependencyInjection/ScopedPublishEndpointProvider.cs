namespace MassTransit.DependencyInjection
{
    using System.Threading.Tasks;


    public class ScopedPublishEndpointProvider<TScope> :
        IPublishEndpointProvider
        where TScope : class
    {
        readonly IPublishEndpointProvider _provider;
        readonly TScope _scope;

        public ScopedPublishEndpointProvider(IPublishEndpointProvider provider, TScope scope)
        {
            _provider = provider;
            _scope = scope;
        }

        ConnectHandle IPublishObserverConnector.ConnectPublishObserver(IPublishObserver observer)
        {
            return _provider.ConnectPublishObserver(observer);
        }

        async Task<ISendEndpoint> IPublishEndpointProvider.GetPublishSendEndpoint<T>()
            where T : class
        {
            var endpoint = await _provider.GetPublishSendEndpoint<T>().ConfigureAwait(false);

            return new ScopedSendEndpoint<TScope>(endpoint, _scope);
        }
    }
}
