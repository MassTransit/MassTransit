namespace MassTransit.ServiceBus.Saga
{
    /// <summary>
    /// Defines a workflow step that handles a specific message type.
    /// </summary>
    /// <typeparam name="T">The type of message to handle.</typeparam>
    /// <remarks>
    /// A workflow is a series of message handlers that are related to each other.
    /// To start a workflow send a message that implements <see cref="ISagaMessage"/>
    /// and implement a handler for that message using IWorkflow&lt;T&gt;.
    /// </remarks>
    public interface ISaga<T> : ISagaController where T : IMessage
    {
        /// <summary>
        /// Handles a workflow message.
        /// </summary>
        /// <param name="message">Th message to handle.</param>
        void Handle(T message);
    }
}