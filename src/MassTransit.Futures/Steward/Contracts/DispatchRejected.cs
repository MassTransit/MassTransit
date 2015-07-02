namespace MassTransit.Steward.Contracts
{
    using System;
    using System.Collections.Generic;


    /// <summary>
    /// Published when a command is rejected due to resource unavailability
    /// </summary>
    public interface DispatchRejected 
    {
        /// <summary>
        /// Uniquely identifies the rejection event
        /// </summary>
        Guid EventId { get; }

        /// <summary>
        /// Uniquely identifies the command to execute
        /// </summary>
        Guid CommandId { get; }

        /// <summary>
        /// The timestamp at which the command execution was requested
        /// </summary>
        DateTime CreateTime { get; }

        /// <summary>
        /// The timestamp at which at the command was rejected
        /// </summary>
        DateTime RejectTime { get; }

        /// <summary>
        /// A list of unavailable resources that caused the command to be rejected
        /// </summary>
        IList<UnavailableResource> UnavailableResources { get; }

        /// <summary>
        /// The message types implemented by the command message
        /// </summary>
        IList<string> CommandType { get; }

        /// <summary>
        /// The destination where the command was sent for execution
        /// </summary>
        Uri Destination { get; }
    }
}