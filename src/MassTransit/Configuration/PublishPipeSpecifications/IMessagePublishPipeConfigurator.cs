namespace MassTransit.PublishPipeSpecifications
{
    using GreenPipes;


    /// <summary>
    /// Configures the Publishing of a message type, allowing filters to be applied
    /// on Publish.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public interface IMessagePublishPipeConfigurator<TMessage> :
        IPipeConfigurator<PublishContext<TMessage>>
        where TMessage : class
    {
    }
}
