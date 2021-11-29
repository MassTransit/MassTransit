namespace MassTransit.Configuration
{
    using System;


    /// <summary>
    /// Connects a message handler to the ConsumePipe
    /// </summary>
    /// <typeparam name="TMessage">The message type</typeparam>
    public interface IObserverConnector<TMessage>
        where TMessage : class
    {
        /// <summary>
        /// Connect a message handler for all messages of type T
        /// </summary>
        /// <param name="consumePipe"></param>
        /// <param name="observer"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        ConnectHandle ConnectObserver(IConsumePipeConnector consumePipe, IObserver<ConsumeContext<TMessage>> observer,
            params IFilter<ConsumeContext<TMessage>>[] filters);

        /// <summary>
        /// Connect a message handler for messages with the specified RequestId
        /// </summary>
        /// <param name="consumePipe"></param>
        /// <param name="requestId"></param>
        /// <param name="observer"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        ConnectHandle ConnectRequestObserver(IRequestPipeConnector consumePipe, Guid requestId, IObserver<ConsumeContext<TMessage>> observer,
            params IFilter<ConsumeContext<TMessage>>[] filters);
    }
}
