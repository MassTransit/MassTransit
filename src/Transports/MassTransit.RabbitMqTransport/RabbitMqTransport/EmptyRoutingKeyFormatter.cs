namespace MassTransit.RabbitMqTransport
{
    public class EmptyRoutingKeyFormatter :
        IRoutingKeyFormatter
    {
        public string FormatRoutingKey<T>(RabbitMqSendContext<T> context)
            where T : class
        {
            return context.RoutingKey;
        }
    }
}
