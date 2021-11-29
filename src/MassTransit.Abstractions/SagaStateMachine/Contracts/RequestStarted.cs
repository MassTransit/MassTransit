namespace MassTransit.Contracts
{
    using System;


    /// <summary>
    /// Published when a saga starts to process a request, but a subsequent operation (such as another request) is
    /// pending.
    /// </summary>
    public interface RequestStarted
    {
        /// <summary>
        /// The saga correlationId, used to reconnect to the saga once the request is completed
        /// </summary>
        Guid CorrelationId { get; }

        /// <summary>
        /// The RequestId header value that was specified in the original request
        /// </summary>
        Guid RequestId { get; }

        /// <summary>
        /// The ResponseAddress header value from the original request
        /// </summary>
        Uri ResponseAddress { get; }

        /// <summary>
        /// The FaultAddress header value from the original request
        /// </summary>
        Uri FaultAddress { get; }

        /// <summary>
        /// The expiration time for this request, which if completed after, the response is discarded
        /// </summary>
        DateTime? ExpirationTime { get; }

        /// <summary>
        /// The payload types supported by the payload
        /// </summary>
        string[] PayloadType { get; }

        /// <summary>
        /// The actual message payload
        /// </summary>
        object Payload { get; }
    }
}
