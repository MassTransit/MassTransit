namespace MassTransit
{
    public static class EventHubConsumeContextExtensions
    {
        public static string PartitionKey(this ConsumeContext context)
        {
            return context.TryGetPayload(out EventHubConsumeContext consumeContext) ? consumeContext.PartitionKey : string.Empty;
        }

        public static long? Offset(this ConsumeContext context)
        {
            return context.TryGetPayload(out EventHubConsumeContext consumeContext) ? consumeContext.Offset : default;
        }

        public static long? SequenceNumber(this ConsumeContext context)
        {
            return context.TryGetPayload(out EventHubConsumeContext consumeContext) ? consumeContext.SequenceNumber : default;
        }
    }
}
