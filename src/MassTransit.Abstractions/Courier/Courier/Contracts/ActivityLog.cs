namespace MassTransit.Courier.Contracts
{
    using System;


    /// <summary>
    /// Message contract for storing activity log data
    /// </summary>
    public interface ActivityLog
    {
        /// <summary>
        /// The tracking number for completion of the activity
        /// </summary>
        Guid ExecutionId { get; }

        /// <summary>
        /// The name of the activity that was completed
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The timestamp when the activity started
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// The duration of the activity execution
        /// </summary>
        TimeSpan Duration { get; }

        /// <summary>
        /// The host that executed the activity
        /// </summary>
        HostInfo Host { get; }
    }
}
