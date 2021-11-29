namespace MassTransit.GrpcTransport
{
    public interface IMessageRoutingKeyFormatter<in TMessage>
        where TMessage : class
    {
        string FormatRoutingKey(GrpcSendContext<TMessage> context);
    }
}
