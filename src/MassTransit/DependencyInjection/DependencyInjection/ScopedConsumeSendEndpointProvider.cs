namespace MassTransit.DependencyInjection
{
    using System;
    using System.Threading.Tasks;


    public class ScopedConsumeSendEndpointProvider<TScope> :
        ISendEndpointProvider
        where TScope : class
    {
        readonly ConsumeContext _consumeContext;
        readonly ISendEndpointProvider _provider;
        readonly TScope _scope;

        public ScopedConsumeSendEndpointProvider(ISendEndpointProvider provider, ConsumeContext consumeContext, TScope scope)
        {
            _provider = provider;
            _consumeContext = consumeContext;
            _scope = scope;
        }

        ConnectHandle ISendObserverConnector.ConnectSendObserver(ISendObserver observer)
        {
            return _provider.ConnectSendObserver(observer);
        }

        async Task<ISendEndpoint> ISendEndpointProvider.GetSendEndpoint(Uri address)
        {
            var endpoint = await _provider.GetSendEndpoint(_consumeContext, address, default).ConfigureAwait(false);

            return new ScopedSendEndpoint<TScope>(endpoint, _scope);
        }
    }
}
