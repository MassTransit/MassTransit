namespace MassTransit.DependencyInjection
{
    using System;
    using System.Threading.Tasks;


    public class ScopedPublishEndpointProvider :
        IPublishEndpointProvider
    {
        readonly IPublishEndpointProvider _provider;
        readonly IServiceProvider _serviceProvider;

        public ScopedPublishEndpointProvider(IPublishEndpointProvider provider, IServiceProvider serviceProvider)
        {
            _provider = provider;
            _serviceProvider = serviceProvider;
        }

        ConnectHandle IPublishObserverConnector.ConnectPublishObserver(IPublishObserver observer)
        {
            return _provider.ConnectPublishObserver(observer);
        }

        async Task<ISendEndpoint> IPublishEndpointProvider.GetPublishSendEndpoint<T>()
            where T : class
        {
            var endpoint = await _provider.GetPublishSendEndpoint<T>().ConfigureAwait(false);

            return new ScopedSendEndpoint(endpoint, _serviceProvider);
        }
    }
}
