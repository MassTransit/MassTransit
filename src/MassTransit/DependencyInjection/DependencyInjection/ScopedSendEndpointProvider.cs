namespace MassTransit.DependencyInjection
{
    using System;
    using System.Threading.Tasks;


    public class ScopedSendEndpointProvider :
        ISendEndpointProvider
    {
        readonly ISendEndpointProvider _provider;
        readonly IServiceProvider _serviceProvider;

        public ScopedSendEndpointProvider(ISendEndpointProvider provider, IServiceProvider serviceProvider)
        {
            _provider = provider;
            _serviceProvider = serviceProvider;
        }

        ConnectHandle ISendObserverConnector.ConnectSendObserver(ISendObserver observer)
        {
            return _provider.ConnectSendObserver(observer);
        }

        async Task<ISendEndpoint> ISendEndpointProvider.GetSendEndpoint(Uri address)
        {
            var endpoint = await _provider.GetSendEndpoint(address).ConfigureAwait(false);

            return new ScopedSendEndpoint(endpoint, _serviceProvider);
        }
    }
}
