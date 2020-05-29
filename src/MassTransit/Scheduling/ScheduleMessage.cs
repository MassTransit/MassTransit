namespace MassTransit.Scheduling
{
    using System;


    public interface ScheduleMessage
    {
        Guid CorrelationId { get; }

        /// <summary>
        /// The time at which the message should be published, should be in UTC
        /// </summary>
        DateTime ScheduledTime { get; }

        /// <summary>
        /// The message types implemented by the message
        /// </summary>
        string[] PayloadType { get; }

        /// <summary>
        /// The destination where the message should be sent
        /// </summary>
        Uri Destination { get; }
    }


    public interface ScheduleMessage<out T> :
        ScheduleMessage
        where T : class
    {
        /// <summary>
        /// The message to be published
        /// </summary>
        T Payload { get; }
    }
}
