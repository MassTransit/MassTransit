namespace MassTransit.DependencyInjection
{
    using System;
    using System.Threading.Tasks;


    public class ScopedConsumePublishEndpointProvider :
        IPublishEndpointProvider
    {
        readonly ConsumeContext _consumeContext;
        readonly IPublishEndpointProvider _provider;
        readonly IServiceProvider _serviceProvider;

        public ScopedConsumePublishEndpointProvider(IPublishEndpointProvider provider, ConsumeContext consumeContext, IServiceProvider serviceProvider)
        {
            _provider = provider;
            _consumeContext = consumeContext;
            _serviceProvider = serviceProvider;
        }

        ConnectHandle IPublishObserverConnector.ConnectPublishObserver(IPublishObserver observer)
        {
            return _provider.ConnectPublishObserver(observer);
        }

        async Task<ISendEndpoint> IPublishEndpointProvider.GetPublishSendEndpoint<T>()
            where T : class
        {
            var endpoint = await _provider.GetPublishEndpoint<T>(_consumeContext, default).ConfigureAwait(false);

            return new ScopedSendEndpoint(endpoint, _serviceProvider);
        }
    }
}
