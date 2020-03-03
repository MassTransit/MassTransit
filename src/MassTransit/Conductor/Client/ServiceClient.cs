namespace MassTransit.Conductor.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Clients;
    using Consumers;
    using Context;
    using GreenPipes;
    using GreenPipes.Caching;


    /// <summary>
    /// Supports linking to a service endpoint, and maintaining the link as service instances go up/down.
    /// </summary>
    public class ServiceClient :
        IServiceClient
    {
        readonly ICache<IServiceClientMessageCache> _cache;
        readonly IIndex<Type, IServiceClientMessageCache> _index;
        readonly IClientFactory _clientFactory;
        readonly CancellationTokenSource _disposed;
        readonly HashSet<ConnectHandle> _handles;
        readonly Guid _clientId;
        readonly IServiceInstanceCache _instanceCache;
        readonly IPublishEndpointProvider _publishEndpointProvider;

        public ServiceClient(IClientFactory clientFactory, IPublishEndpointProvider publishEndpointProvider)
        {
            _clientFactory = clientFactory;
            _publishEndpointProvider = publishEndpointProvider;

            _clientId = NewId.NextGuid();

            _disposed = new CancellationTokenSource();
            _handles = new HashSet<ConnectHandle>();

            _cache = new GreenCache<IServiceClientMessageCache>(ServiceClientCacheDefaults.Settings);
            _index = _cache.AddIndex("messageType", x => x.MessageType);

            _instanceCache = new ServiceInstanceCache();

            ConnectConsumers();
        }

        public IRequestSendEndpoint<T> CreateRequestSendEndpoint<T>(ConsumeContext consumeContext)
            where T : class
        {
            if (consumeContext == null)
                throw new ArgumentNullException(nameof(consumeContext));

            Task<IServiceClientMessageCache<T>> messageClient = GetMessageClient<T>();

            return new ServiceClientRequestSendEndpoint<T>(messageClient, _clientFactory.Context, consumeContext);
        }

        public IRequestSendEndpoint<T> CreateRequestSendEndpoint<T>()
            where T : class
        {
            Task<IServiceClientMessageCache<T>> messageClient = GetMessageClient<T>();

            return new ServiceClientRequestSendEndpoint<T>(messageClient, _clientFactory.Context);
        }

        async Task IAsyncDisposable.DisposeAsync(CancellationToken cancellationToken)
        {
            _disposed.Cancel();

            var clients = _cache.GetAll();

            async Task DisposeClient(Task<IServiceClientMessageCache> client)
            {
                try
                {
                    var messageClient = await client.ConfigureAwait(false);

                    await messageClient.UnlinkClient(_publishEndpointProvider).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    LogContext.Warning?.Log(exception, "MessageClient Dispose faulted: (client-id: {ClientId})", _clientId);
                }
            }

            await Task.WhenAll(clients.Select(DisposeClient)).ConfigureAwait(false);

            _cache.Clear();

            foreach (var handle in _handles)
                handle.Disconnect();

            await _clientFactory.DisposeAsync(cancellationToken).ConfigureAwait(false);
        }

        async Task<IServiceClientMessageCache<T>> GetMessageClient<T>()
            where T : class
        {
            if (_disposed.IsCancellationRequested)
                throw new ObjectDisposedException("The service client has been disposed.");

            var client = await _index.Get(typeof(T), type => CreateMessageClient<T>(_disposed.Token)).ConfigureAwait(false);

            return client as IServiceClientMessageCache<T>;
        }

        async Task<IServiceClientMessageCache> CreateMessageClient<T>(CancellationToken cancellationToken)
            where T : class
        {
            var messageClient = new ServiceClientMessageCache<T>(_clientFactory, _clientId, _instanceCache);

            var handle = _clientFactory.Context.ConnectInstance(messageClient);
            try
            {
                await messageClient.Link(cancellationToken).ConfigureAwait(false);

                _handles.Add(handle);

                return messageClient;
            }
            catch (Exception)
            {
                handle.Disconnect();
                throw;
            }
        }

        void ConnectConsumers()
        {
            _handles.Add(_clientFactory.Context.ConnectConsumer(() => new InstanceDownConsumer(_instanceCache)));
        }
    }
}
