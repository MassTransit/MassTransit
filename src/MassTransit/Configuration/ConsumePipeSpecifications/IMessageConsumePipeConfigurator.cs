namespace MassTransit.ConsumePipeSpecifications
{
    using GreenPipes;


    /// <summary>
    /// Configures the Consumeing of a message type, allowing filters to be applied
    /// on Consume.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public interface IMessageConsumePipeConfigurator<TMessage> :
        IPipeConfigurator<ConsumeContext<TMessage>>
        where TMessage : class
    {
    }
}
