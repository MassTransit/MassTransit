namespace MassTransit.Contracts
{
    using System;


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

        /// <summary>
        /// The actual message payload
        /// </summary>
        object Payload { get; }
    }
}
