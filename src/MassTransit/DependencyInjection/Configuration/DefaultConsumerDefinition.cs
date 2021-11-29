namespace MassTransit.Configuration
{
    /// <summary>
    /// A default consumer definition, used if no definition is found for the consumer type
    /// </summary>
    /// <typeparam name="TConsumer"></typeparam>
    public class DefaultConsumerDefinition<TConsumer> :
        ConsumerDefinition<TConsumer>
        where TConsumer : class, IConsumer
    {
    }
}
