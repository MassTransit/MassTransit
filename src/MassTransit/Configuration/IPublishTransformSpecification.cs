namespace MassTransit
{
    using GreenPipes;


    public interface IPublishTransformSpecification<TMessage> :
        IPipeSpecification<PublishContext<TMessage>>
        where TMessage : class
    {
    }
}
