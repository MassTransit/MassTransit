namespace MassTransit.Conductor.Client
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Clients;
    using GreenPipes;


    /// <summary>
    /// The service client factory will use the <see cref="ServiceClient"/> for requests which do not specify a destination address. If a destination address
    /// is specified, the call will be delegated to the regular client factory.
    /// </summary>
    public class ServiceClientFactory :
        IClientFactory
    {
        readonly IClientFactory _clientFactory;
        readonly ClientFactoryContext _context;
        readonly IServiceClient _serviceClient;

        public ServiceClientFactory(IServiceClient serviceClient, IClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            _context = _clientFactory.Context;
            _serviceClient = serviceClient;
        }

        ClientFactoryContext IClientFactory.Context => _context;

        public RequestHandle<T> CreateRequest<T>(T message, CancellationToken cancellationToken, RequestTimeout timeout)
            where T : class
        {
            IRequestClient<T> client = CreateRequestClient<T>(timeout);

            return client.Create(message, cancellationToken);
        }

        public RequestHandle<T> CreateRequest<T>(Uri destinationAddress, T message, CancellationToken cancellationToken, RequestTimeout timeout)
            where T : class
        {
            return _clientFactory.CreateRequest(destinationAddress, message, cancellationToken, timeout);
        }

        public RequestHandle<T> CreateRequest<T>(ConsumeContext consumeContext, T message, CancellationToken cancellationToken, RequestTimeout timeout)
            where T : class
        {
            IRequestClient<T> client = CreateRequestClient<T>(consumeContext, timeout);

            return client.Create(message, cancellationToken);
        }

        public RequestHandle<T> CreateRequest<T>(ConsumeContext consumeContext, Uri destinationAddress, T message, CancellationToken cancellationToken,
            RequestTimeout timeout)
            where T : class
        {
            return _clientFactory.CreateRequest(consumeContext, destinationAddress, message, cancellationToken, timeout);
        }

        public IRequestClient<T> CreateRequestClient<T>(RequestTimeout timeout)
            where T : class
        {
            IRequestSendEndpoint<T> requestSendEndpoint = _serviceClient.CreateRequestSendEndpoint<T>();

            return new RequestClient<T>(_context, requestSendEndpoint, timeout.Or(_context.DefaultTimeout));
        }

        public IRequestClient<T> CreateRequestClient<T>(ConsumeContext consumeContext, RequestTimeout timeout)
            where T : class
        {
            IRequestSendEndpoint<T> requestSendEndpoint = _serviceClient.CreateRequestSendEndpoint<T>(consumeContext);

            return new RequestClient<T>(_context, requestSendEndpoint, timeout.Or(_context.DefaultTimeout));
        }

        public IRequestClient<T> CreateRequestClient<T>(Uri destinationAddress, RequestTimeout timeout)
            where T : class
        {
            return _clientFactory.CreateRequestClient<T>(destinationAddress, timeout);
        }

        public IRequestClient<T> CreateRequestClient<T>(ConsumeContext consumeContext, Uri destinationAddress, RequestTimeout timeout)
            where T : class
        {
            return _clientFactory.CreateRequestClient<T>(consumeContext, destinationAddress, timeout);
        }

        async Task IAsyncDisposable.DisposeAsync(CancellationToken cancellationToken)
        {
            await _serviceClient.DisposeAsync(cancellationToken).ConfigureAwait(false);
            await _clientFactory.DisposeAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
