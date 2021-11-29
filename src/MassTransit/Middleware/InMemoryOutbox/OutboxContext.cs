namespace MassTransit.Middleware.InMemoryOutbox
{
    using System;
    using System.Threading.Tasks;


    /// <summary>
    /// The context for an outbox instance as part of consume context. Used to signal the completion of
    /// the consume, and store any Task factories that should be created.
    /// </summary>
    public interface OutboxContext
    {
        /// <summary>
        /// Returns an awaitable task that is completed when it is clear to send messages
        /// </summary>
        Task ClearToSend { get; }

        /// <summary>
        /// Adds a method to be invoked once the outbox is ready to be sent
        /// </summary>
        /// <param name="method"></param>
        Task Add(Func<Task> method);

        /// <summary>
        /// Execute all the pending outbox operations (success case)
        /// </summary>
        /// <param name="concurrentMessageDelivery"></param>
        /// <returns></returns>
        Task ExecutePendingActions(bool concurrentMessageDelivery);

        /// <summary>
        /// Discard any pending outbox operations, and cancel any scheduled messages
        /// </summary>
        /// <returns></returns>
        Task DiscardPendingActions();
    }
}
