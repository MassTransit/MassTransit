namespace MassTransit.MessageData.Configuration
{
    public interface IMessageDataTransformConfiguration<TInput>
        where TInput : class
    {
        void Apply(ITransformConfigurator<TInput> configurator);
    }
}
