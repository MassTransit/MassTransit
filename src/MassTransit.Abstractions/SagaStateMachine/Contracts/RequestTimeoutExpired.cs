namespace MassTransit.Contracts
{
    using System;


    public interface RequestTimeoutExpired<TRequest>
        where TRequest : class
    {
        /// <summary>
        /// The correlationId of the state machine
        /// </summary>
        Guid CorrelationId { get; }

        /// <summary>
        /// When the request expired
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// The expiration time that was scheduled for the request
        /// </summary>
        DateTime ExpirationTime { get; }

        /// <summary>
        /// The requestId of the request
        /// </summary>
        Guid RequestId { get; }
    }
}
