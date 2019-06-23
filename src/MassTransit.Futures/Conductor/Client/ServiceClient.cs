namespace MassTransit.Conductor.Client
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Clients;
    using Context;
    using Contracts;
    using GreenPipes;
    using GreenPipes.Caching;
    using MassTransit.Pipeline;
    using Util;


    public class ServiceClient :
        IServiceClient
    {
        readonly ICache<IMessageClient> _cache;
        readonly IIndex<Type, IMessageClient> _index;
        readonly IClientFactory _clientFactory;
        readonly IConsumePipeConnector _consumePipeConnector;
        readonly IClientFactory _serviceClientFactory;

        public ServiceClient(IClientFactory clientFactory, ClientFactoryContext clientFactoryContext, IConsumePipeConnector consumePipeConnector)
        {
            _clientFactory = clientFactory;
            _consumePipeConnector = consumePipeConnector;

            ClientId = NewId.NextGuid();

            _serviceClientFactory = new ServiceClientFactory(this, clientFactoryContext, clientFactory);

            var cacheSettings = new CacheSettings(1000, TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(30));
            _cache = new GreenCache<IMessageClient>(cacheSettings);
            _index = _cache.AddIndex("messageType", x => x.MessageType);
        }

        public Guid ClientId { get; }

        public async Task<ISendEndpoint> GetServiceSendEndpoint<T>(ISendEndpointProvider sendEndpointProvider, CancellationToken cancellationToken)
            where T : class
        {
            var messageClient = await _index.Get(typeof(T), type => CreateMessageClient<T>(cancellationToken)).ConfigureAwait(false);

            return await messageClient.GetServiceSendEndpoint(sendEndpointProvider).ConfigureAwait(false);
        }

        async Task<IMessageClient> CreateMessageClient<T>(CancellationToken cancellationToken)
            where T : class
        {
            var messageClient = new MessageClient<T>(this);

            var handle = _consumePipeConnector.ConnectInstance(messageClient);
            try
            {
                LogContext.Debug?.Log("Requesting Link to {MessageType} (client-id: {ClientId})", TypeMetadataCache<T>.ShortName, ClientId);

                var request = _clientFactory.CreateRequestClient<Link<T>>().Create(new {ClientId}, cancellationToken);

                var response = await request.GetResponse<Up<T>>().ConfigureAwait(false);

                return messageClient;
            }
            catch (Exception)
            {
                handle.Disconnect();
                throw;
            }
        }

        Task IAsyncDisposable.DisposeAsync(CancellationToken cancellationToken)
        {
            _cache.Clear();

            return _clientFactory.DisposeAsync(cancellationToken);
        }

        RequestHandle<T> IClientFactory.CreateRequest<T>(T message, CancellationToken cancellationToken, RequestTimeout timeout)
        {
            return _serviceClientFactory.CreateRequest(message, cancellationToken, timeout);
        }

        RequestHandle<T> IClientFactory.CreateRequest<T>(Uri destinationAddress, T message, CancellationToken cancellationToken, RequestTimeout timeout)
        {
            return _serviceClientFactory.CreateRequest(destinationAddress, message, cancellationToken, timeout);
        }

        RequestHandle<T> IClientFactory.CreateRequest<T>(ConsumeContext consumeContext, T message, CancellationToken cancellationToken, RequestTimeout timeout)
        {
            return _serviceClientFactory.CreateRequest(consumeContext, message, cancellationToken, timeout);
        }

        RequestHandle<T> IClientFactory.CreateRequest<T>(ConsumeContext consumeContext, Uri destinationAddress, T message, CancellationToken cancellationToken,
            RequestTimeout timeout)
        {
            return _serviceClientFactory.CreateRequest(consumeContext, destinationAddress, message, cancellationToken, timeout);
        }

        IRequestClient<T> IClientFactory.CreateRequestClient<T>(RequestTimeout timeout)
        {
            return _serviceClientFactory.CreateRequestClient<T>(timeout);
        }

        IRequestClient<T> IClientFactory.CreateRequestClient<T>(ConsumeContext consumeContext, RequestTimeout timeout)
        {
            return _serviceClientFactory.CreateRequestClient<T>(consumeContext, timeout);
        }

        IRequestClient<T> IClientFactory.CreateRequestClient<T>(Uri destinationAddress, RequestTimeout timeout)
        {
            return _serviceClientFactory.CreateRequestClient<T>(destinationAddress, timeout);
        }

        IRequestClient<T> IClientFactory.CreateRequestClient<T>(ConsumeContext consumeContext, Uri destinationAddress, RequestTimeout timeout)
        {
            return _serviceClientFactory.CreateRequestClient<T>(consumeContext, destinationAddress, timeout);
        }
    }
}
