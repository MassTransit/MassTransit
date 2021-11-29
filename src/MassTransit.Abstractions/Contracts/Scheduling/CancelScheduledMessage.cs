namespace MassTransit.Scheduling
{
    using System;


    public interface CancelScheduledMessage
    {
        /// <summary>
        /// The cancel scheduled message correlationId
        /// </summary>
        Guid CorrelationId { get; }

        /// <summary>
        /// The date/time this message was created
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// The token of the scheduled message
        /// </summary>
        Guid TokenId { get; }
    }
}
