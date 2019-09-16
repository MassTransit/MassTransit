namespace MassTransit.Conductor.Pipeline
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using Metadata;
    using Server;
    using Util;


    public class ConductorMessageFilter<TMessage> :
        IFilter<ConsumeContext<TMessage>>
        where TMessage : class
    {
        readonly IMessageEndpoint<TMessage> _endpoint;

        public ConductorMessageFilter(IMessageEndpoint<TMessage> endpoint)
        {
            _endpoint = endpoint;
        }

        public async Task Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
        {
            if (context.RequestId.HasValue)
            {
                var clientId = context.Headers.Get(MessageHeaders.ClientId, default(Guid?));
                if (clientId.HasValue)
                {
                    var acceptedContext = await _endpoint.Accept(clientId.Value, context.RequestId.Value).ConfigureAwait(false);

                    context.AddOrUpdatePayload(() => acceptedContext, existing => acceptedContext);

                    await next.Send(context).ConfigureAwait(false);
                }
                else
                {
                    var exception = new RequestException($"ClientId not specified (requestId: {context.RequestId})");

                    await context.NotifyFaulted(TimeSpan.Zero, TypeMetadataCache<ConductorMessageFilter<TMessage>>.ShortName, exception).ConfigureAwait(false);
                }
            }
            else
            {
                var exception = new RequestException("RequestId is required");

                await context.NotifyFaulted(TimeSpan.Zero, TypeMetadataCache<ConductorMessageFilter<TMessage>>.ShortName, exception).ConfigureAwait(false);
            }
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("conductorRequest");
        }
    }
}
