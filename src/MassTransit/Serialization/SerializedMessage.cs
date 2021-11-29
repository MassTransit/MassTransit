namespace MassTransit.Serialization
{
    using System;


    /// <summary>
    /// The content of a serialized message
    /// </summary>
    public interface SerializedMessage
    {
        /// <summary>
        /// The destination for the serialized message
        /// </summary>
        Uri Destination { get; }

        /// <summary>
        /// The content type of the serializer used
        /// </summary>
        string ContentType { get; }

        string ExpirationTime { get; }
        string ResponseAddress { get; }
        string FaultAddress { get; }
        string Body { get; }
        string MessageId { get; }
        string RequestId { get; }
        string CorrelationId { get; }
        string ConversationId { get; }
        string InitiatorId { get; }
        string HeadersAsJson { get; }
        string PayloadMessageHeadersAsJson { get; }
    }
}
