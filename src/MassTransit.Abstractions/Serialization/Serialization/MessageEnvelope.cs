namespace MassTransit.Serialization
{
    using System;
    using System.Collections.Generic;


    public interface MessageEnvelope
    {
        string? MessageId { get; }
        string? RequestId { get; }
        string? CorrelationId { get; }
        string? ConversationId { get; }
        string? InitiatorId { get; }
        string? SourceAddress { get; }
        string? DestinationAddress { get; }
        string? ResponseAddress { get; }
        string? FaultAddress { get; }
        string[]? MessageType { get; }
        object? Message { get; }
        DateTime? ExpirationTime { get; }
        DateTime? SentTime { get; }
        Dictionary<string, object?>? Headers { get; }
        HostInfo? Host { get; }
    }
}
