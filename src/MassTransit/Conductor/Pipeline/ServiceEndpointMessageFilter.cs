namespace MassTransit.Conductor.Pipeline
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using Server;


    /// <summary>
    /// Adds the client context to the payload, if ClientId is present. May reject messages if ClientId is blocked.
    /// Adds the client to the cache if not present.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public class ServiceEndpointMessageFilter<TMessage> :
        IFilter<ConsumeContext<TMessage>>
        where TMessage : class
    {
        readonly IServiceEndpointMessageClientCache _clientCache;

        public ServiceEndpointMessageFilter(IServiceEndpointMessageClientCache clientCache)
        {
            _clientCache = clientCache;
        }

        public async Task Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
        {
            if (context.Headers.TryGetHeader(MessageHeaders.ClientId, out var obj)
                && (obj is Guid clientId || obj is string s && Guid.TryParse(s, out clientId)))
            {
                var clientContext = await _clientCache.Link(clientId, context.ResponseAddress ?? context.SourceAddress).ConfigureAwait(false);

                clientContext.NotifyConsumed(context);

                context.GetOrAddPayload(() => clientContext);
            }

            await next.Send(context).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("serviceEndpoint");
        }
    }
}
