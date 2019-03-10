namespace Automatonymous.Contracts
{
    using System;
    using MassTransit;


    /// <summary>
    /// Published when a request faults, including the fault info available
    /// </summary>
    public interface RequestFaulted<out T> :
        RequestFaulted
        where T : class
    {
        Fault<T> Payload { get; }
    }


    public interface RequestFaulted
    {
        /// <summary>
        /// The saga correlationId, used to reconnect to the saga once the request is completed
        /// </summary>
        Guid CorrelationId { get; }

        /// <summary>
        /// The payload types supported by the payload
        /// </summary>
        string[] PayloadType { get; }
    }
}
