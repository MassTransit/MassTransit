namespace MassTransit.RabbitMqTransport.Topology.Conventions.RoutingKey
{
    public class EmptyRoutingKeyFormatter :
        IRoutingKeyFormatter
    {
        string IRoutingKeyFormatter.FormatRoutingKey<T>(RabbitMqSendContext<T> context)
        {
            return context.RoutingKey;
        }
    }
}
