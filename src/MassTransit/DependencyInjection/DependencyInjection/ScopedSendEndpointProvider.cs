namespace MassTransit.DependencyInjection
{
    using System;
    using System.Threading.Tasks;


    public class ScopedSendEndpointProvider<TScope> :
        ISendEndpointProvider
        where TScope : class
    {
        readonly ISendEndpointProvider _provider;
        readonly TScope _scope;

        public ScopedSendEndpointProvider(ISendEndpointProvider provider, TScope scope)
        {
            _provider = provider;
            _scope = scope;
        }

        ConnectHandle ISendObserverConnector.ConnectSendObserver(ISendObserver observer)
        {
            return _provider.ConnectSendObserver(observer);
        }

        async Task<ISendEndpoint> ISendEndpointProvider.GetSendEndpoint(Uri address)
        {
            var endpoint = await _provider.GetSendEndpoint(address).ConfigureAwait(false);

            return new ScopedSendEndpoint<TScope>(endpoint, _scope);
        }
    }
}
