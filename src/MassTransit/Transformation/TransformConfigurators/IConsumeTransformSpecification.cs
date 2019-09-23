namespace MassTransit.Transformation.TransformConfigurators
{
    using GreenPipes;


    public interface IConsumeTransformSpecification<TMessage> :
        IPipeSpecification<ConsumeContext<TMessage>>
        where TMessage : class
    {
    }
}
