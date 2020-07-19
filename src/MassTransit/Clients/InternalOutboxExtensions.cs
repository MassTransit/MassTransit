namespace MassTransit.Clients
{
    using Context;
    using Pipeline.Filters.Outbox;


    static class InternalOutboxExtensions
    {
        internal static ISendEndpoint SkipOutbox(this ISendEndpoint endpoint)
        {
            if (endpoint is ConsumeSendEndpoint consumeSendEndpoint)
                endpoint = consumeSendEndpoint.Endpoint;

            if (endpoint is OutboxSendEndpoint outboxSendEndpoint)
                return outboxSendEndpoint.Endpoint;

            return endpoint;
        }
    }
}
