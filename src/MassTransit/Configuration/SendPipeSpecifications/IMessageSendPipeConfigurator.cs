namespace MassTransit.SendPipeSpecifications
{
    using GreenPipes;


    /// <summary>
    /// Configures the sending of a message type, allowing filters to be applied
    /// on send.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public interface IMessageSendPipeConfigurator<TMessage> :
        IPipeConfigurator<SendContext<TMessage>>
        where TMessage : class
    {
    }
}
