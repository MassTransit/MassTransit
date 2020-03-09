namespace MassTransit.Conductor.Client
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Clients;
    using Context;
    using Contexts;
    using Contracts;
    using Distribution;
    using GreenPipes.Caching;
    using GreenPipes.Internals.Extensions;
    using Metadata;
    using Util;


    public class ServiceClientMessageCache<TMessage> :
        IServiceClientMessageCache<TMessage>,
        IConsumer<Up<TMessage>>,
        ICacheValueObserver<ServiceInstanceContext>
        where TMessage : class
    {
        readonly IClientFactory _clientFactory;
        readonly IServiceInstanceCache _instanceCache;
        readonly IDistributionStrategy<ServiceInstanceContext> _distribution;
        readonly TaskCompletionSource<Uri> _serviceAddress;

        public ServiceClientMessageCache(IClientFactory clientFactory, Guid clientId, IServiceInstanceCache instanceCache)
        {
            _clientFactory = clientFactory;
            _instanceCache = instanceCache;

            ClientId = clientId;

            _serviceAddress = TaskUtil.GetTask<Uri>();

            _distribution = new ConsistentHashDistributionStrategy<ServiceInstanceContext>(new Murmur3AUnsafeHashGenerator(), GetHashKey);
            _distribution.Init(Enumerable.Empty<ServiceInstanceContext>());

            instanceCache.Connect(this);
        }

        public async Task Consume(ConsumeContext<Up<TMessage>> context)
        {
            var instance = context.Message.Instance;
            var instanceId = instance.InstanceId;

            var serviceAddress = context.Message.Service.ServiceAddress;

            LogContext.Debug?.Log("Up: {ServiceAddress} {InstanceId}", serviceAddress, instanceId);

            var instanceContext = await _instanceCache.GetOrAdd(instanceId, instance).ConfigureAwait(false);

            _distribution.Add(instanceContext);

            _serviceAddress.TrySetResult(serviceAddress);
        }

        public Guid ClientId { get; }
        public Type MessageType => typeof(TMessage);

        public async Task UnlinkClient(IPublishEndpointProvider publishEndpointProvider)
        {
            try
            {
                var sendEndpoint = await publishEndpointProvider.GetPublishSendEndpoint<Unlink<TMessage>>().ConfigureAwait(false);

                await sendEndpoint.Send<Unlink<TMessage>>(new {ClientId}).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                LogContext.Warning?.Log(exception, "Failed to unlink client: {MessageType}", TypeMetadataCache<TMessage>.ShortName);
            }
        }

        public async Task<IRequestSendEndpoint<TMessage>> GetServiceSendEndpoint(ClientFactoryContext clientFactoryContext, TMessage message,
            ConsumeContext consumeContext = default, CancellationToken cancellationToken = default)
        {
            // TODO use the message to generate the hash key

            // var correlationId = NewId.NextGuid();
            //
            // var endpointInfo = _distribution.GetNode(correlationId.ToByteArray());
            // if (endpointInfo == null)
            // {
            //     await Link(cancellationToken).ConfigureAwait(false);
            //
            //     endpointInfo = _distribution.GetNode(correlationId.ToByteArray());
            // }

            var endpointAddress = _serviceAddress.Task.IsCompletedSuccessfully()
                ? _serviceAddress.Task.Result
                : await _serviceAddress.Task.ConfigureAwait(false);

            // TODO may want to switch this up so that it includes the InstanceContext is a specific node selected
            return clientFactoryContext.GetRequestEndpoint<TMessage>(endpointAddress, consumeContext);
        }

        static byte[] GetHashKey(ServiceInstanceContext context)
        {
            return context.InstanceId.ToByteArray();
        }

        public async Task Link(CancellationToken cancellationToken)
        {
            LogContext.Debug?.Log("Linking: {ClientId} {MessageType}", ClientId, TypeMetadataCache<TMessage>.ShortName);

            var client = _clientFactory.CreateRequestClient<Link<TMessage>>();

            var response = await client.GetResponse<Up<TMessage>>(new {ClientId}, cancellationToken).ConfigureAwait(false);

            var serviceAddress = response.Message.Service.ServiceAddress;

            var instance = response.Message.Instance;

            var instanceContext = await _instanceCache.GetOrAdd(instance.InstanceId, instance).ConfigureAwait(false);

            _distribution.Add(instanceContext);

            LogContext.Debug?.Log("Linked: {ClientId} {MessageType} {ServiceAddress}", ClientId, TypeMetadataCache<TMessage>.ShortName, serviceAddress);

            _serviceAddress.TrySetResult(serviceAddress);
        }


        class EndpointTracker
        {
            public EndpointTracker(InstanceInfo nodeInfo, Guid correlationId)
            {
                LastCorrelationId = correlationId.ToNewId();
                NodeInfo = nodeInfo;
            }

            public NewId LastCorrelationId { get; }
            public InstanceInfo NodeInfo { get; }
        }


        public void ValueAdded(INode<ServiceInstanceContext> node, ServiceInstanceContext value)
        {
        }

        public void ValueRemoved(INode<ServiceInstanceContext> node, ServiceInstanceContext value)
        {
            _distribution.Remove(value);
        }

        public void CacheCleared()
        {
        }
    }
}
