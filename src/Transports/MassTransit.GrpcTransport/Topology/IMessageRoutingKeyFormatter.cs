namespace MassTransit.GrpcTransport.Topology
{
    public interface IMessageRoutingKeyFormatter<in TMessage>
        where TMessage : class
    {
        string FormatRoutingKey(GrpcSendContext<TMessage> context);
    }
}
