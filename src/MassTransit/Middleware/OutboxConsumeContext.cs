namespace MassTransit.Middleware
{
    using System.Collections.Generic;
    using System.Threading.Tasks;


    public interface OutboxConsumeContext :
        ConsumeContext,
        OutboxSendContext
    {
        ConsumeContext CapturedContext { get; }

        /// <summary>
        /// If true, continue processing
        /// </summary>
        bool ContinueProcessing { set; }

        /// <summary>
        /// If true, the message was already consumed
        /// </summary>
        bool IsMessageConsumed { get; }

        /// <summary>
        /// If true, the outbox messages have already been dispatched
        /// </summary>
        bool IsOutboxDelivered { get; }

        /// <summary>
        /// The number of delivery attempts for this message
        /// </summary>
        int ReceiveCount { get; }

        /// <summary>
        /// The last sequence number produced from the outbox
        /// </summary>
        long? LastSequenceNumber { get; }

        Task SetConsumed();

        Task SetDelivered();

        Task<List<OutboxMessageContext>> LoadOutboxMessages();

        Task NotifyOutboxMessageDelivered(OutboxMessageContext message);

        Task RemoveOutboxMessages();
    }


    public interface OutboxConsumeContext<out TMessage> :
        OutboxConsumeContext,
        ConsumeContext<TMessage>
        where TMessage : class
    {
    }
}
