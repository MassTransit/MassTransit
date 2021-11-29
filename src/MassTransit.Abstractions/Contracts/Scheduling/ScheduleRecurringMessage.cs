namespace MassTransit.Scheduling
{
    using System;


    public interface ScheduleRecurringMessage
    {
        Guid CorrelationId { get; }

        RecurringSchedule Schedule { get; }

        /// <summary>
        /// The message types implemented by the message
        /// </summary>
        string[] PayloadType { get; }

        /// <summary>
        /// The destination where the message should be sent
        /// </summary>
        Uri Destination { get; }

        /// <summary>
        /// The actual scheduled message payload
        /// </summary>
        object Payload { get; }
    }
}
