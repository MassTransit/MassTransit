namespace MassTransit
{
    using Azure.ServiceBus.Core;


    public static class BrokeredMessageContextExtensions
    {
        public static string SessionId(this ConsumeContext context)
        {
            return context.TryGetPayload<BrokeredMessageContext>(out var brokeredMessageContext) ? brokeredMessageContext.SessionId : string.Empty;
        }

        public static string PartitionKey(this ConsumeContext context)
        {
            return context.TryGetPayload<BrokeredMessageContext>(out var brokeredMessageContext) ? brokeredMessageContext.PartitionKey : string.Empty;
        }

        public static string ReplyToSessionId(this ConsumeContext context)
        {
            return context.TryGetPayload<BrokeredMessageContext>(out var brokeredMessageContext) ? brokeredMessageContext.ReplyToSessionId : string.Empty;
        }

        public static string ViaPartitionKey(this ConsumeContext context)
        {
            return context.TryGetPayload<BrokeredMessageContext>(out var brokeredMessageContext) ? brokeredMessageContext.ViaPartitionKey : string.Empty;
        }
    }
}
