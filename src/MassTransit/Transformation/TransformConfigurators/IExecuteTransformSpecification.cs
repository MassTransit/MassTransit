namespace MassTransit.Transformation.TransformConfigurators
{
    using Courier;
    using GreenPipes;


    public interface IExecuteTransformSpecification<TArguments> :
        IPipeSpecification<ExecuteContext<TArguments>>
        where TArguments : class
    {
    }
}
