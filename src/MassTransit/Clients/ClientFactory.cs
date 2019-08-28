namespace MassTransit.Clients
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Util;


    public class ClientFactory :
        IClientFactory
    {
        readonly ClientFactoryContext _context;

        public ClientFactory(ClientFactoryContext context)
        {
            _context = context;
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
            IRequestClient<T> client = CreateRequestClient<T>(destinationAddress, timeout);

            return client.Create(message, cancellationToken);
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
            IRequestClient<T> client = CreateRequestClient<T>(consumeContext, destinationAddress, timeout);

            return client.Create(message, cancellationToken);
        }

        public IRequestClient<T> CreateRequestClient<T>(RequestTimeout timeout)
            where T : class
        {
            if (EndpointConvention.TryGetDestinationAddress<T>(out var destinationAddress))
                return CreateRequestClient<T>(destinationAddress, timeout);

            return new RequestClient<T>(_context, new PublishRequestSendEndpoint<T>(_context.PublishEndpoint), timeout.Or(_context.DefaultTimeout));
        }

        public IRequestClient<T> CreateRequestClient<T>(ConsumeContext consumeContext, RequestTimeout timeout)
            where T : class
        {
            if (EndpointConvention.TryGetDestinationAddress<T>(out var destinationAddress))
                return CreateRequestClient<T>(consumeContext, destinationAddress, timeout);

            return new RequestClient<T>(_context, new PublishRequestSendEndpoint<T>(consumeContext), timeout.Or(_context.DefaultTimeout));
        }

        public IRequestClient<T> CreateRequestClient<T>(Uri destinationAddress, RequestTimeout timeout)
            where T : class
        {
            var requestSendEndpoint = new SendRequestSendEndpoint<T>(_context, destinationAddress);

            return new RequestClient<T>(_context, requestSendEndpoint, timeout.Or(_context.DefaultTimeout));
        }

        public IRequestClient<T> CreateRequestClient<T>(ConsumeContext consumeContext, Uri destinationAddress, RequestTimeout timeout)
            where T : class
        {
            var requestSendEndpoint = new SendRequestSendEndpoint<T>(consumeContext, destinationAddress);

            return new RequestClient<T>(_context, requestSendEndpoint, timeout.Or(_context.DefaultTimeout));
        }

        Task IAsyncDisposable.DisposeAsync(CancellationToken cancellationToken)
        {
            if (_context is IAsyncDisposable asyncDisposable)
                return asyncDisposable.DisposeAsync(cancellationToken);

            return TaskUtil.Completed;
        }
    }
}
