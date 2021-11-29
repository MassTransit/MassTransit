namespace MassTransit
{
    public static class ServiceBusMessageContextExtensions
    {
        public static string SessionId(this ConsumeContext context)
        {
            return context.TryGetPayload<ServiceBusMessageContext>(out var brokeredMessageContext) ? brokeredMessageContext.SessionId : string.Empty;
        }

        public static string PartitionKey(this ConsumeContext context)
        {
            return context.TryGetPayload<ServiceBusMessageContext>(out var brokeredMessageContext) ? brokeredMessageContext.PartitionKey : string.Empty;
        }

        public static string ReplyToSessionId(this ConsumeContext context)
        {
            return context.TryGetPayload<ServiceBusMessageContext>(out var brokeredMessageContext) ? brokeredMessageContext.ReplyToSessionId : string.Empty;
        }
    }
}
