namespace MassTransit.Middleware
{
    public interface IMessageSendPipe<in TMessage> :
        IPipe<SendContext<TMessage>>
        where TMessage : class
    {
    }
}
