namespace MassTransit.Conductor.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Contracts;
    using Distribution;
    using GreenPipes;
    using GreenPipes.Caching;
    using Metadata;
    using Util;


    public class MessageClient<TMessage> :
        IMessageClient<TMessage>,
        IConsumer<Up<TMessage>>,
        IConsumer<Down<TMessage>>
        where TMessage : class
    {
        readonly ICache<EndpointInfo> _cache;
        readonly IClientFactory _clientFactory;
        readonly IDistributionStrategy<EndpointInfo> _distribution;
        readonly IIndex<Guid, EndpointInfo> _index;
        readonly TaskCompletionSource<Uri> _serviceAddress;

        public MessageClient(IClientFactory clientFactory, Guid clientId)
        {
            _clientFactory = clientFactory;

            ClientId = clientId;

            _serviceAddress = TaskUtil.GetTask<Uri>();

            var cacheSettings = new CacheSettings(1000, TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(30));
            _cache = new GreenCache<EndpointInfo>(cacheSettings);
            _index = _cache.AddIndex("endpointId", x => x.EndpointId);

            _distribution = new ConsistentHashDistributionStrategy<EndpointInfo>(new Murmur3AUnsafeHashGenerator(), GetHashKey);
            _distribution.Init(Enumerable.Empty<EndpointInfo>());
        }

        public async Task Consume(ConsumeContext<Down<TMessage>> context)
        {
            LogContext.Debug?.Log("Endpoint Down received: (service-address: {ServiceAddress}, endpoint-address: {EndpointAddress}, client-id: {ClientId})",
                context.Message.ServiceAddress, context.Message.Endpoint.EndpointAddress, ClientId);

            try
            {
                var existing = await _index.Get(context.Message.Endpoint.EndpointId).ConfigureAwait(false);
                if (context.SentTime >= existing.Started)
                {
                    _distribution.Remove(context.Message.Endpoint);

                    _index.Remove(context.Message.Endpoint.EndpointId);
                }
            }
            catch (KeyNotFoundException)
            {
            }
        }

        public async Task Consume(ConsumeContext<Up<TMessage>> context)
        {
            LogContext.Debug?.Log("Endpoint Up received: (service-address: {ServiceAddress}, endpoint-address: {EndpointAddress}, client-id: {ClientId})",
                context.Message.ServiceAddress, context.Message.Endpoint.EndpointAddress, ClientId);

            _distribution.Add(context.Message.Endpoint);

            await _index.Get(context.Message.Endpoint.EndpointId, id => Task.FromResult(context.Message.Endpoint)).ConfigureAwait(false);

            _serviceAddress.TrySetResult(context.Message.ServiceAddress);
        }

        public Guid ClientId { get; }
        public Type MessageType => typeof(TMessage);

        public async Task<ISendEndpoint> GetServiceSendEndpoint(ISendEndpointProvider sendEndpointProvider, TMessage message,
            CancellationToken cancellationToken = default)
        {
            // TODO use the message to generate the hash key

            var correlationId = NewId.NextGuid();

            var endpointInfo = _distribution.GetNode(correlationId.ToByteArray());
            if (endpointInfo == null)
            {
                await Link(cancellationToken).ConfigureAwait(false);

                endpointInfo = _distribution.GetNode(correlationId.ToByteArray());
            }

            var endpointAddress = endpointInfo?.EndpointAddress ?? await _serviceAddress.Task.ConfigureAwait(false);

            return await sendEndpointProvider.GetSendEndpoint(endpointAddress).ConfigureAwait(false);
        }

        async Task IAsyncDisposable.DisposeAsync(CancellationToken cancellationToken)
        {
            // say goodbye to service endpoints?
        }

        static byte[] GetHashKey(EndpointInfo endpointInfo)
        {
            return endpointInfo.EndpointId.ToByteArray();
        }

        public async Task Link(CancellationToken cancellationToken)
        {
            LogContext.Debug?.Log("Requesting Link to {MessageType} (client-id: {ClientId})", TypeMetadataCache<TMessage>.ShortName, ClientId);

            using (RequestHandle<Link<TMessage>> request = _clientFactory.CreateRequestClient<Link<TMessage>>().Create(new {ClientId}, cancellationToken))
            {
                Response<Up<TMessage>> response = await request.GetResponse<Up<TMessage>>().ConfigureAwait(false);

                _distribution.Add(response.Message.Endpoint);

                await _index.Get(response.Message.Endpoint.EndpointId, id => Task.FromResult(response.Message.Endpoint)).ConfigureAwait(false);

                LogContext.Debug?.Log("Linked to {InstanceAddress} for {MessageType} (client-id: {ClientId})",
                    response.Message.Endpoint.InstanceAddress, TypeMetadataCache<TMessage>.ShortName, ClientId);
            }
        }


        class EndpointTracker
        {
            public EndpointTracker(EndpointInfo endpointInfo, Guid correlationId)
            {
                LastCorrelationId = correlationId.ToNewId();
                EndpointInfo = endpointInfo;
            }

            public NewId LastCorrelationId { get; }
            public EndpointInfo EndpointInfo { get; }
        }
    }
}
