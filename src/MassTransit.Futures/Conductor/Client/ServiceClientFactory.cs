namespace MassTransit.Conductor.Client
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Clients;
    using GreenPipes;
    using Util;


    public class ServiceClientFactory :
        IClientFactory
    {
        readonly IServiceClient _serviceClient;
        readonly IClientFactory _clientFactory;
        readonly ClientFactoryContext _context;

        public ServiceClientFactory(IServiceClient serviceClient, ClientFactoryContext context, IClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            _context = context;
            _serviceClient = serviceClient;
        }

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
            var requestSendEndpoint = new ServiceClientRequestSendEndpoint<T>(_serviceClient, _context);

            return new RequestClient<T>(_context, requestSendEndpoint, timeout.Or(_context.DefaultTimeout));
        }

        public IRequestClient<T> CreateRequestClient<T>(ConsumeContext consumeContext, RequestTimeout timeout)
            where T : class
        {
            var requestSendEndpoint = new ServiceClientRequestSendEndpoint<T>(_serviceClient, consumeContext);

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

        Task IAsyncDisposable.DisposeAsync(CancellationToken cancellationToken)
        {
            if (_context is IAsyncDisposable asyncDisposable)
                return asyncDisposable.DisposeAsync(cancellationToken);

            return TaskUtil.Completed;
        }
    }
}
