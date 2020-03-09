namespace MassTransit.Conductor.Consumers
{
    using System.Threading.Tasks;
    using Context;
    using Contracts;
    using Server;


    public class LinkConsumer<TMessage> :
        IConsumer<Link<TMessage>>
        where TMessage : class
    {
        readonly IServiceEndpointMessageClientCache _clientCache;
        readonly ServiceInfo _serviceInfo;
        readonly InstanceInfo _instanceInfo;

        public LinkConsumer(IServiceEndpointMessageClientCache clientCache, ServiceInfo serviceInfo, InstanceInfo instanceInfo)
        {
            _clientCache = clientCache;
            _serviceInfo = serviceInfo;
            _instanceInfo = instanceInfo;
        }

        public async Task Consume(ConsumeContext<Link<TMessage>> context)
        {
            LogContext.Debug?.Log("Link: {ClientId}", context.Message.ClientId);

            var clientAddress = context.ResponseAddress ?? context.SourceAddress;

            var clientContext = await _clientCache.Link(context.Message.ClientId, clientAddress).ConfigureAwait(false);

            clientContext.NotifyConsumed(context);

            await context.RespondAsync<Up<TMessage>>(new
            {
                Service = _serviceInfo,
                Instance = _instanceInfo,
                Message = _clientCache.MessageInfo
            }).ConfigureAwait(false);
        }
    }
}
