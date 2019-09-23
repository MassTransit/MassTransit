namespace MassTransit.Transformation.TransformConfigurators
{
    public interface IMessageDataTransformConfiguration<TInput>
        where TInput : class
    {
        void Apply(ITransformConfigurator<TInput> configurator);
    }
}
