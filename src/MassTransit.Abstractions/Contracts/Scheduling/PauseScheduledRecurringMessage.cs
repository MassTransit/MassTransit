using System;


namespace MassTransit.Scheduling
{
    public interface PauseScheduledRecurringMessage
    {
        /// <summary>
        /// The cancel scheduled message correlationId
        /// </summary>
        Guid CorrelationId { get; }

        /// <summary>
        /// The date/time this message was created
        /// </summary>
        DateTime Timestamp { get; }

        string ScheduleId { get; }

        string ScheduleGroup { get; }
    }
}
