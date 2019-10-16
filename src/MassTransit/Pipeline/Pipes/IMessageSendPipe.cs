namespace MassTransit.Pipeline.Pipes
{
    using GreenPipes;


    public interface IMessageSendPipe<in TMessage> :
        IPipe<SendContext<TMessage>>
        where TMessage : class
    {
    }
}
