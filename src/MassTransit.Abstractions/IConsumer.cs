namespace MassTransit
{
    using System.Threading.Tasks;


    /// <summary>
    /// Defines a class that is a consumer of a message. The message is wrapped in an IConsumeContext
    /// interface to allow access to details surrounding the inbound message, including headers.
    /// </summary>
    /// <typeparam name="TMessage">The message type</typeparam>
    public interface IConsumer<in TMessage> :
        IConsumer
        where TMessage : class
    {
        Task Consume(ConsumeContext<TMessage> context);
    }


    /// <summary>
    /// Marker interface used to assist identification in IoC containers.
    /// Not to be used directly as it does not contain the message type of the
    /// consumer
    /// </summary>
    /// <remarks>
    /// Not to be used directly by application code, for internal reflection only
    /// </remarks>
    public interface IConsumer
    {
    }
}
