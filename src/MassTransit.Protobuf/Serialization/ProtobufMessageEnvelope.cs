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
    [ProtoContract]
    [ProtoInclude(100, typeof(HostInfo))]
    [ProtoInclude(101, typeof(ProtobufHostInfo))]
    public class ProtobufMessageEnvelope<TProtoMessage>
        where TProtoMessage : class
    {
        [ProtoMember(1)]
        public string? MessageId { get; set; }
        [ProtoMember(2)]
        public string? RequestId { get; set; }
        [ProtoMember(3)]
        public string? CorrelationId { get; set; }
        [ProtoMember(4)]
        public string? ConversationId { get; set; }
        [ProtoMember(5)]
        public string? InitiatorId { get; set; }
        [ProtoMember(6)]
        public string? SourceAddress { get; set; }
        [ProtoMember(7)]
        public string? DestinationAddress { get; set; }
        [ProtoMember(8)]
        public string? ResponseAddress { get; set; }
        [ProtoMember(9)]
        public string? FaultAddress { get; set; }
        [ProtoMember(10)]
        public string[]? MessageType { get; set; }
        [ProtoMember(11)]
        public TProtoMessage? _message;
        [ProtoMember(12)]
        public DateTime? ExpirationTime { get; set; }
        [ProtoMember(13)]
        public DateTime? SentTime { get; set; }
        [ProtoMember(14)]
        public ProtobufHostInfo? _host;

        [ProtoMember(15)]
        Dictionary<string, TProtoMessage?>? _headers { get; set; }

        public HostInfo? Host
        {
            get => _host;
            set => _host = ToProtobufHostInfo(value!);
        }

        public object? Message
        {
            get => _message as object;
            set => _message = (TProtoMessage?)value;
        }

        public Dictionary<string, object?>? Headers
        {
            get => _headers?.ToDictionary(kv => kv.Key, kv => kv.Value as object);
            set => _headers = value?.ToDictionary(kv => kv.Key, kv => kv.Value as TProtoMessage);
        }

        private ProtobufHostInfo ToProtobufHostInfo(HostInfo hostInfo)
        {
            return new ProtobufHostInfo()
            {
                MachineName = hostInfo.MachineName,
                ProcessName = hostInfo.ProcessName,
                ProcessId = hostInfo.ProcessId,
                Assembly = hostInfo.Assembly,
                AssemblyVersion = hostInfo.AssemblyVersion,
                FrameworkVersion = hostInfo.FrameworkVersion,
                MassTransitVersion = hostInfo.MassTransitVersion,
                OperatingSystemVersion = hostInfo.OperatingSystemVersion
            };
        }

        public ProtobufMessageEnvelope()
        {
        }

        public ProtobufMessageEnvelope(SendContext context, TProtoMessage message, string[] messageTypeNames)
        {
            if (context.MessageId.HasValue)
                MessageId = context.MessageId.Value.ToString();
            if (context.RequestId.HasValue)
                RequestId = context.RequestId.Value.ToString();
            if (context.CorrelationId.HasValue)
                CorrelationId = context.CorrelationId.Value.ToString();
            if (context.ConversationId.HasValue)
                ConversationId = context.ConversationId.Value.ToString();
            if (context.InitiatorId.HasValue)
                InitiatorId = context.InitiatorId.Value.ToString();
            if (context.SourceAddress != null)
                SourceAddress = context.SourceAddress.ToString();
            if (context.DestinationAddress != null)
                DestinationAddress = context.DestinationAddress.ToString();
            if (context.ResponseAddress != null)
                ResponseAddress = context.ResponseAddress.ToString();
            if (context.FaultAddress != null)
                FaultAddress = context.FaultAddress.ToString();
            MessageType = messageTypeNames;
            Message = message as TProtoMessage;
            if (context.TimeToLive.HasValue)
                ExpirationTime = DateTime.UtcNow + context.TimeToLive;
            SentTime = context.SentTime ?? DateTime.UtcNow;
            Headers = new Dictionary<string, object?>();
            foreach (KeyValuePair<string, object> header in context.Headers.GetAll())
                Headers[header.Key] = header.Value as TProtoMessage;
            Host = HostMetadataCache.Host;
        }

        public ProtobufMessageEnvelope(MessageContext context, TProtoMessage message, string[] messageTypeNames)
        {
            if (context.MessageId.HasValue)
                MessageId = context.MessageId.Value.ToString();
            if (context.RequestId.HasValue)
                RequestId = context.RequestId.Value.ToString();
            if (context.CorrelationId.HasValue)
                CorrelationId = context.CorrelationId.Value.ToString();
            if (context.ConversationId.HasValue)
                ConversationId = context.ConversationId.Value.ToString();
            if (context.InitiatorId.HasValue)
                InitiatorId = context.InitiatorId.Value.ToString();
            if (context.SourceAddress != null)
                SourceAddress = context.SourceAddress.ToString();
            if (context.DestinationAddress != null)
                DestinationAddress = context.DestinationAddress.ToString();
            if (context.ResponseAddress != null)
                ResponseAddress = context.ResponseAddress.ToString();
            if (context.FaultAddress != null)
                FaultAddress = context.FaultAddress.ToString();
            MessageType = messageTypeNames;
            Message = message as TProtoMessage;
            if (context.ExpirationTime.HasValue)
                ExpirationTime = context.ExpirationTime;
            SentTime = context.SentTime ?? DateTime.UtcNow;
            Headers = new Dictionary<string, object?>();
            foreach (KeyValuePair<string, object> header in context.Headers.GetAll())
                Headers[header.Key] = header.Value as TProtoMessage;
            Host = HostMetadataCache.Host;
        }

        public ProtobufMessageEnvelope(MessageEnvelope envelope)
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
            Message = envelope.Message as TProtoMessage;
            ExpirationTime = envelope.ExpirationTime;
            SentTime = envelope.SentTime ?? DateTime.UtcNow;
            Headers = envelope.Headers != null
                ? new Dictionary<string, object?>(envelope.Headers, StringComparer.OrdinalIgnoreCase) as Dictionary<string, object?>
                : new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
            Host = envelope.Host ?? HostMetadataCache.Host;
        }

        public void Update(SendContext context)
        {
            DestinationAddress = context.DestinationAddress?.ToString();

            if (context.SourceAddress != null)
                SourceAddress = context.SourceAddress.ToString();

            if (context.ResponseAddress != null)
                ResponseAddress = context.ResponseAddress.ToString();

            if (context.FaultAddress != null)
                FaultAddress = context.FaultAddress.ToString();

            if (context.MessageId.HasValue)
                MessageId = context.MessageId.ToString();

            if (context.RequestId.HasValue)
                RequestId = context.RequestId.ToString();

            if (context.ConversationId.HasValue)
                ConversationId = context.ConversationId.ToString();

            if (context.CorrelationId.HasValue)
                CorrelationId = context.CorrelationId.ToString();

            if (context.InitiatorId.HasValue)
                InitiatorId = context.InitiatorId.ToString();

            if (context.TimeToLive.HasValue)
                ExpirationTime = DateTime.UtcNow + (context.TimeToLive > TimeSpan.Zero ? context.TimeToLive : TimeSpan.FromSeconds(1));

            foreach (KeyValuePair<string, object> header in context.Headers.GetAll())
                if (Headers != null)
                    Headers[header.Key] = header.Value as TProtoMessage;
                else
                    Headers = new Dictionary<string, object?> { { header.Key, header.Value as TProtoMessage } };
        }
    }
}
