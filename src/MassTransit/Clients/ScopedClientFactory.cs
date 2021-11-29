namespace MassTransit.Clients
{
    using System;
    using System.Threading;


    public class ScopedClientFactory :
        IScopedClientFactory
    {
        readonly IClientFactory _clientFactory;
        readonly ConsumeContext _consumeContext;

        public ScopedClientFactory(IClientFactory clientFactory, ConsumeContext consumeContext)
        {
            _clientFactory = clientFactory;
            _consumeContext = consumeContext;
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
            IRequestClient<T> client = CreateRequestClient<T>(destinationAddress, timeout);

            return client.Create(message, cancellationToken);
        }

        public RequestHandle<T> CreateRequest<T>(object values, CancellationToken cancellationToken, RequestTimeout timeout)
            where T : class
        {
            IRequestClient<T> client = CreateRequestClient<T>(timeout);

            return client.Create(values, cancellationToken);
        }

        public RequestHandle<T> CreateRequest<T>(Uri destinationAddress, object values, CancellationToken cancellationToken, RequestTimeout timeout)
            where T : class
        {
            IRequestClient<T> client = CreateRequestClient<T>(destinationAddress, timeout);

            return client.Create(values, cancellationToken);
        }

        public IRequestClient<T> CreateRequestClient<T>(RequestTimeout timeout)
            where T : class
        {
            if (EndpointConvention.TryGetDestinationAddress<T>(out var destinationAddress))
                return CreateRequestClient<T>(destinationAddress, timeout);

            return _clientFactory.CreateRequestClient<T>(_consumeContext, timeout);
        }

        public IRequestClient<T> CreateRequestClient<T>(Uri destinationAddress, RequestTimeout timeout)
            where T : class
        {
            return _clientFactory.CreateRequestClient<T>(_consumeContext, destinationAddress, timeout);
        }
    }
}
