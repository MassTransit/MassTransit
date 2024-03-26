namespace MassTransit.Middleware
{
    using InMemoryOutbox;
    using Transports;


    public static class InternalOutboxExtensions
    {
        internal static ISendEndpoint SkipOutbox(this ISendEndpoint endpoint)
        {
            if (endpoint is ConsumeSendEndpoint consumeSendEndpoint)
                endpoint = consumeSendEndpoint.Endpoint;

            if (endpoint is OutboxSendEndpoint outboxSendEndpoint)
                endpoint = outboxSendEndpoint.Endpoint;

            if (endpoint is Outbox.OutboxSendEndpoint outboxEndpoint)
                return outboxEndpoint.Endpoint;

            return endpoint;
        }

        public static ConsumeContext SkipOutbox(ConsumeContext context)
        {
            while (context.TryGetPayload<InMemoryOutboxConsumeContext>(out var outboxConsumeContext))
                context = outboxConsumeContext.CapturedContext;

            while (context.TryGetPayload<OutboxConsumeContext>(out var outboxConsumeContext))
                context = outboxConsumeContext.CapturedContext;

            return context;
        }
    }
}
