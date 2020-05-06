namespace MassTransit.ExtensionsDependencyInjectionIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Clients;


    public interface IClientFactory<in TBus> :
        IClientFactory
        where TBus : class
    {
    }


    class ClientFactory<TBus> :
        IClientFactory<TBus>
        where TBus : class
    {
        readonly IClientFactory _clientFactory;

        public Task DisposeAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return _clientFactory.DisposeAsync(cancellationToken);
        }

        public RequestHandle<T> CreateRequest<T>(T message, CancellationToken cancellationToken = default, RequestTimeout timeout = default)
            where T : class
        {
            return _clientFactory.CreateRequest(message, cancellationToken, timeout);
        }

        public RequestHandle<T> CreateRequest<T>(Uri destinationAddress, T message, CancellationToken cancellationToken = default,
            RequestTimeout timeout = default)
            where T : class
        {
            return _clientFactory.CreateRequest(destinationAddress, message, cancellationToken, timeout);
        }

        public RequestHandle<T> CreateRequest<T>(ConsumeContext consumeContext, T message, CancellationToken cancellationToken = default,
            RequestTimeout timeout = default)
            where T : class
        {
            return _clientFactory.CreateRequest(consumeContext, message, cancellationToken, timeout);
        }

        public RequestHandle<T> CreateRequest<T>(ConsumeContext consumeContext, Uri destinationAddress, T message,
            CancellationToken cancellationToken = default,
            RequestTimeout timeout = default)
            where T : class
        {
            return _clientFactory.CreateRequest(consumeContext, destinationAddress, message, cancellationToken, timeout);
        }

        public IRequestClient<T> CreateRequestClient<T>(RequestTimeout timeout = default)
            where T : class
        {
            return _clientFactory.CreateRequestClient<T>(timeout);
        }

        public IRequestClient<T> CreateRequestClient<T>(ConsumeContext consumeContext, RequestTimeout timeout = default)
            where T : class
        {
            return _clientFactory.CreateRequestClient<T>(consumeContext, timeout);
        }

        public IRequestClient<T> CreateRequestClient<T>(Uri destinationAddress, RequestTimeout timeout = default)
            where T : class
        {
            return _clientFactory.CreateRequestClient<T>(destinationAddress, timeout);
        }

        public IRequestClient<T> CreateRequestClient<T>(ConsumeContext consumeContext, Uri destinationAddress, RequestTimeout timeout = default)
            where T : class
        {
            return _clientFactory.CreateRequestClient<T>(consumeContext, destinationAddress, timeout);
        }

        public ClientFactoryContext Context => _clientFactory.Context;

        public ClientFactory(IClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }
    }
}
