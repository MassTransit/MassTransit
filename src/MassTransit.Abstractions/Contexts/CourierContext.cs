namespace MassTransit
{
    using System;
    using Courier.Contracts;


    public interface CourierContext :
        ConsumeContext<RoutingSlip>
    {
        /// <summary>
        /// The tracking number for this routing slip
        /// </summary>
        Guid TrackingNumber { get; }

        /// <summary>
        /// The name of the activity
        /// </summary>
        string ActivityName { get; }

        /// <summary>
        /// The executionId for this attempt at the activity
        /// </summary>
        Guid ExecutionId { get; }

        /// <summary>
        /// The start time for the activity execution
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// The time elapsed for the execution operation
        /// </summary>
        TimeSpan Elapsed { get; }
    }
}
