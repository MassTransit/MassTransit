namespace MassTransit.Definition
{
    public interface IConsumerMessageDefinition<TConsumer, TMessage>
        where TConsumer : class, IConsumer<TMessage>
        where TMessage : class
    {
    }
}
