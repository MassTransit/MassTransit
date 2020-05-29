namespace MassTransit
{
    using GreenPipes;


    public interface ISendTransformSpecification<TMessage> :
        IPipeSpecification<SendContext<TMessage>>
        where TMessage : class
    {
    }
}
