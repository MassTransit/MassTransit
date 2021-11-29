namespace MassTransit.Configuration
{
    public interface IConsumeTransformSpecification<TMessage> :
        IPipeSpecification<ConsumeContext<TMessage>>
        where TMessage : class
    {
    }
}
