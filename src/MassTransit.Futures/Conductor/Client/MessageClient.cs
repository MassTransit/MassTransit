namespace MassTransit.Conductor.Client
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Context;
    using Contracts;
    using Distribution;
    using GreenPipes.Caching;
    using Util;


    public class MessageClient<TMessage> :
        IMessageClient<TMessage>,
        IConsumer<Up<TMessage>>
        where TMessage : class
    {
        readonly IServiceClient _serviceClient;
        readonly ICache<EndpointInfo> _cache;
        readonly IIndex<Guid, EndpointInfo> _index;
        readonly TaskCompletionSource<Uri> _serviceAddress;
        readonly IDistributionStrategy<EndpointInfo> _distribution;

        public MessageClient(IServiceClient serviceClient)
        {
            _serviceClient = serviceClient;

            _serviceAddress = new TaskCompletionSource<Uri>();

            var cacheSettings = new CacheSettings(1000, TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(30));
            _cache = new GreenCache<EndpointInfo>(cacheSettings);
            _index = _cache.AddIndex("endpointId", x => x.EndpointId);

            _distribution = new ConsistentHashDistributionStrategy<EndpointInfo>(new Murmur3AUnsafeHashGenerator(), GetHashKey);
            _distribution.Init(Enumerable.Empty<EndpointInfo>());
        }

        static string GetHashKey(EndpointInfo endpointInfo)
        {
            return FormatUtil.Formatter.Format(endpointInfo.EndpointId.ToByteArray());
        }

        public Type MessageType => typeof(TMessage);

        public async Task<ISendEndpoint> GetServiceSendEndpoint(ISendEndpointProvider sendEndpointProvider)
        {
            Uri address = await _serviceAddress.Task.ConfigureAwait(false);

            var endpoint = _distribution.GetNode(NewId.NextGuid().ToByteArray());

            return await sendEndpointProvider.GetSendEndpoint(address).ConfigureAwait(false);
        }

        public async Task Consume(ConsumeContext<Up<TMessage>> context)
        {
            LogContext.Debug?.Log("EndpointInfo received: (service-address: {ServiceAddress}, endpoint-address: {EndpointAddress}, client-id: {ClientId})",
                context.Message.ServiceAddress, context.Message.Endpoint.EndpointAddress, _serviceClient.ClientId);

            _distribution.Add(context.Message.Endpoint);

            await _index.Get(context.Message.Endpoint.EndpointId, id => Task.FromResult(context.Message.Endpoint));

            _serviceAddress.TrySetResult(context.Message.ServiceAddress);
        }
    }
}
