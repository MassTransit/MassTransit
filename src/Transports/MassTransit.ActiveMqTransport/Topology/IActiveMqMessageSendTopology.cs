namespace MassTransit
{
    public interface IActiveMqMessageSendTopology<TMessage> :
        IMessageSendTopology<TMessage>
        where TMessage : class
    {
    }
}
