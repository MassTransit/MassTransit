namespace MassTransit.Pipeline.Pipes
{
    using GreenPipes;


    public interface IMessagePublishPipe<in TMessage> :
        IPipe<PublishContext<TMessage>>
        where TMessage : class
    {
    }
}
