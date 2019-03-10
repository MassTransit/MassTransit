namespace Automatonymous.Contracts
{
    using System;


    /// <summary>
    /// Published by the saga when a request is completed, so that waiting requests can be completed, or redelivered to the
    /// saga for completion.
    /// </summary>
    public interface RequestCompleted
    {
        /// <summary>
        /// The saga correlationId
        /// </summary>
        Guid CorrelationId { get; }

        /// <summary>
        /// The timestamp when the request was completed
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// The payload types supported by the payload
        /// </summary>
        string[] PayloadType { get; }
    }


    /// <summary>
    /// Published by the saga when a request is completed, so that waiting requests can be completed, or redelivered to the
    /// saga for completion.
    /// </summary>
    /// <typeparam name="TResponse">The response type</typeparam>
    public interface RequestCompleted<out TResponse> :
        RequestCompleted
        where TResponse : class
    {
        TResponse Payload { get; }
    }
}
