namespace MassTransit.Configuration
{
    public interface ISendTransformSpecification<TMessage> :
        IPipeSpecification<SendContext<TMessage>>
        where TMessage : class
    {
    }
}
