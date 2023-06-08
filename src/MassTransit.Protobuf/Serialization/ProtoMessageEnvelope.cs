#nullable enable
namespace MassTransit.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MassTransit;
    using Metadata;
    using ProtoBuf;

    [Serializable]
    public class ProtoMessageEnvelope<TProtoMessage> : MessageEnvelope
        where TProtoMessage : class
    {
        public string? MessageId { get; set; }
        public string? RequestId { get; set; }
        public string? CorrelationId { get; set; }
        public string? ConversationId { get; set; }
        public string? InitiatorId { get; set; }
        public string? SourceAddress { get; set; }
        public string? DestinationAddress { get; set; }
        public string? ResponseAddress { get; set; }
        public string? FaultAddress { get; set; }
        public string[]? MessageType { get; set; }
        public object? Message { get; set; }
        public DateTime? ExpirationTime { get; set; }
        public DateTime? SentTime { get; set; }
        public HostInfo? Host { get; set; }
        public Dictionary<string, object?>? Headers { get; }

        public ProtoMessageEnvelope(ProtobufMessageEnvelope<TProtoMessage> envelope)
        {
            MessageId = envelope.MessageId;
            RequestId = envelope.RequestId;
            CorrelationId = envelope.CorrelationId;
            ConversationId = envelope.ConversationId;
            InitiatorId = envelope.InitiatorId;
            SourceAddress = envelope.SourceAddress;
            DestinationAddress = envelope.DestinationAddress;
            ResponseAddress = envelope.ResponseAddress;
            FaultAddress = envelope.FaultAddress;
            MessageType = envelope.MessageType;
            Message = envelope.Message;
            ExpirationTime = envelope.ExpirationTime;
            SentTime = envelope.SentTime;
            Headers = envelope.Headers;
        }
    }
}
