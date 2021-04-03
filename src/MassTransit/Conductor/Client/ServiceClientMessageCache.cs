namespace MassTransit.Conductor.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Clients;
    using Context;
    using Contexts;
    using Contracts.Conductor;
    using Distribution;
    using GreenPipes.Caching;
    using Metadata;


    public class ServiceClientMessageCache<TMessage> :
        IServiceClientMessageCache<TMessage>,
        IConsumer<Up<TMessage>>,
        ICacheValueObserver<ServiceInstanceContext>
        where TMessage : class
    {
        readonly IClientFactory _clientFactory;
        readonly IDistributionStrategy<ServiceInstanceContext, TMessage> _distribution;
        readonly IServiceInstanceCache _instanceCache;

        public ServiceClientMessageCache(IClientFactory clientFactory, Guid clientId, IServiceInstanceCache instanceCache, IDistributionStrategy<ServiceInstanceContext, TMessage> distributionStrategy = default)
        {
            _clientFactory = clientFactory;
            _instanceCache = instanceCache;

            ClientId = clientId;

            if (distributionStrategy == default)
            {
                //_distribution = new ConsistentHashDistributionStrategy<ServiceInstanceContext>(new Murmur3AUnsafeHashGenerator(), GetHashKey);
                _distribution = new RoundRobinDistributionStrategy();
            }
            else
            {
                _distribution = distributionStrategy;
            }

            _distribution.Init(Enumerable.Empty<ServiceInstanceContext>());

            instanceCache.Connect(this);
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

        public async Task Consume(ConsumeContext<Up<TMessage>> context)
        {
            var instance = context.Message.Instance;
            var instanceId = instance.InstanceId;

            var serviceAddress = context.Message.Service.ServiceAddress;

            LogContext.Debug?.Log("Up: {ServiceAddress} {InstanceId}", serviceAddress, instanceId);

            var instanceContext = await _instanceCache.GetOrAdd(instanceId, serviceAddress, instance).ConfigureAwait(false);

            _distribution.Add(instanceContext);
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
            var instanceContext = await _distribution.GetNode(message);
            if (instanceContext == null)
            {
                await Link(cancellationToken).ConfigureAwait(false);

                instanceContext = await _distribution.GetNode(message);
            }

            if (instanceContext == null)
            {
                throw new MassTransitException("No nodes available");
            }

            // TODO may want to switch this up so that it includes the InstanceContext is a specific node selected
            return clientFactoryContext.GetRequestEndpoint<TMessage>(instanceContext.Endpoint, consumeContext);
        }

        public async Task<IRequestSendEndpoint<TMessage>> GetServiceSendEndpoint(ClientFactoryContext clientFactoryContext, object values,
            ConsumeContext consumeContext = default, CancellationToken cancellationToken = default)
        {
            // TODO implement proper distrbution here too instead of this workarond
            var instanceContext = (await _distribution.GetAvailableNodes()).FirstOrDefault();

            if (instanceContext == null)
            {
                throw new MassTransitException("No nodes available");
            }

            // TODO may want to switch this up so that it includes the InstanceContext is a specific node selected
            return clientFactoryContext.GetRequestEndpoint<TMessage>(instanceContext.Endpoint, consumeContext);
        }

        static byte[] GetHashKey(ServiceInstanceContext context)
        {
            return context.InstanceId.ToByteArray();
        }

        public async Task Link(CancellationToken cancellationToken)
        {
            LogContext.Debug?.Log("Linking: {ClientId} {MessageType}", ClientId, TypeMetadataCache<TMessage>.ShortName);

            IRequestClient<Link<TMessage>> client = _clientFactory.CreateRequestClient<Link<TMessage>>();

            Response<Up<TMessage>> response = await client.GetResponse<Up<TMessage>>(new {ClientId}, cancellationToken).ConfigureAwait(false);

            var serviceAddress = response.Message.Service.ServiceAddress;

            var instance = response.Message.Instance;

            var instanceContext = await _instanceCache.GetOrAdd(instance.InstanceId, serviceAddress, instance).ConfigureAwait(false);

            _distribution.Add(instanceContext);

            LogContext.Debug?.Log("Linked: {ClientId} {MessageType} {ServiceAddress}", ClientId, TypeMetadataCache<TMessage>.ShortName, serviceAddress);
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
    }
}
