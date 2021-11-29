namespace MassTransit
{
    public interface IAmazonSqsMessageSendTopology<TMessage> :
        IMessageSendTopology<TMessage>
        where TMessage : class
    {
    }
}
