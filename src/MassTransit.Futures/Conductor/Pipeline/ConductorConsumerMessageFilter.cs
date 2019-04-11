namespace MassTransit.Conductor.Pipeline
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using Server;


    public class ConductorConsumerMessageFilter<TConsumer, TMessage> :
        IFilter<ConsumerConsumeContext<TConsumer, TMessage>>
        where TMessage : class
        where TConsumer : class
    {
        readonly IMessageEndpoint<TMessage> _endpoint;

        public ConductorConsumerMessageFilter(IMessageEndpoint<TMessage> endpoint)
        {
            _endpoint = endpoint;
        }

        public async Task Send(ConsumerConsumeContext<TConsumer, TMessage> context, IPipe<ConsumerConsumeContext<TConsumer, TMessage>> next)
        {
            if (!context.RequestId.HasValue)
                throw new RequestException("RequestId is required");

            var clientId = context.Headers.Get(MessageHeaders.ClientId, default(Guid?));
            if (!clientId.HasValue)
                throw new RequestException($"ClientId not specified (requestId: {context.RequestId})");

            var acceptedContext = await _endpoint.Accept(clientId.Value, context.RequestId.Value).ConfigureAwait(false);

            context.AddOrUpdatePayload(() => acceptedContext, existing => acceptedContext);

            await next.Send(context).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("consumerLimit");
        }
    }
}