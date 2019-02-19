namespace MassTransit.Definition
{
    public interface IConsumerMessagePropertyDefinitionConfigurator<TConsumer, TMessage, TProperty>
        where TConsumer : class, IConsumer
        where TMessage : class
    {
    }
}
