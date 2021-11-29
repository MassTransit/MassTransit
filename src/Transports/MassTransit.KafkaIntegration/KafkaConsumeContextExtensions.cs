namespace MassTransit
{
    public static class KafkaConsumeContextExtensions
    {
        public static int? Partition(this ConsumeContext context)
        {
            return context.TryGetPayload(out KafkaConsumeContext consumeContext) ? consumeContext.Partition : default;
        }

        public static long? Offset(this ConsumeContext context)
        {
            return context.TryGetPayload(out KafkaConsumeContext consumeContext) ? consumeContext.Offset : default;
        }

        public static TKey GetKey<TKey>(this ConsumeContext context)
        {
            return context.TryGetPayload(out KafkaConsumeContext<TKey> consumeContext) ? consumeContext.Key : default;
        }
    }
}
