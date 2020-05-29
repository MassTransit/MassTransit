namespace MassTransit
{
    using RabbitMqTransport;


    public static class RabbitMqBasicConsumeContextExtensions
    {
        public static string RoutingKey(this ConsumeContext context)
        {
            return context.TryGetPayload<RabbitMqBasicConsumeContext>(out var consumeContext) ? consumeContext.RoutingKey : string.Empty;
        }

        public static string RoutingKey(this SendContext context)
        {
            return context.TryGetPayload<RabbitMqSendContext>(out var sendContext) ? sendContext.RoutingKey : string.Empty;
        }
    }
}
