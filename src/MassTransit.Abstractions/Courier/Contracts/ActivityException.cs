namespace MassTransit.Courier.Contracts
{
    using System;


    /// <summary>
    /// Capture the exception information thrown by an activity
    /// </summary>
    public interface ActivityException
    {
        /// <summary>
        /// The tracking number of the activity that threw the exception
        /// </summary>
        Guid ExecutionId { get; }

        /// <summary>
        /// The point in time when the exception occurred
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// The time from when the routing slip was created until the exception occurred
        /// </summary>
        TimeSpan Elapsed { get; }

        /// <summary>
        /// The name of the activity that caused the exception
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The host where the exception was thrown
        /// </summary>
        HostInfo Host { get; }

        /// <summary>
        /// The exception details
        /// </summary>
        ExceptionInfo ExceptionInfo { get; }
    }
}
