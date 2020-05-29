namespace MassTransit.RabbitMqTransport.Topology
{
    public interface IMessageRoutingKeyFormatter<in TMessage>
        where TMessage : class
    {
        string FormatRoutingKey(RabbitMqSendContext<TMessage> context);
    }
}
