namespace MassTransit.Transformation.TransformConfigurators
{
    using Courier;
    using GreenPipes;


    public interface ICompensateTransformSpecification<TLog> :
        IPipeSpecification<CompensateContext<TLog>>
        where TLog : class
    {
    }
}
