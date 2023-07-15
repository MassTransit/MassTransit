namespace MassTransit.Transports
{
    public interface IMessageRoutingKeyFormatter<in TMessage>
        where TMessage : class
    {
        string FormatRoutingKey(SendContext<TMessage> context);
    }
}
