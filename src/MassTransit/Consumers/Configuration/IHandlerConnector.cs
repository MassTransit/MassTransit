namespace MassTransit.Configuration
{
    using System;


    /// <summary>
    /// Connects a message handler to the ConsumePipe
    /// </summary>
    /// <typeparam name="T">The message type</typeparam>
    public interface IHandlerConnector<T>
        where T : class
    {
        /// <summary>
        /// Connect a message handler for all messages of type T
        /// </summary>
        ConnectHandle ConnectHandler(IConsumePipeConnector consumePipe, MessageHandler<T> handler,
            IBuildPipeConfigurator<ConsumeContext<T>> configurator);

        /// <summary>
        /// Connect a message handler for messages with the specified RequestId
        /// </summary>
        ConnectHandle ConnectRequestHandler(IRequestPipeConnector consumePipe, Guid requestId, MessageHandler<T> handler,
            IBuildPipeConfigurator<ConsumeContext<T>> configurator);
    }
}
