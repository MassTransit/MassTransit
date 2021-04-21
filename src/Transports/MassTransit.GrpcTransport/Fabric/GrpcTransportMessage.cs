namespace MassTransit.GrpcTransport.Fabric
{
    using System;
    using System.Linq;
    using Context;
    using Contracts;
    using Integration;


    public class GrpcTransportMessage :
        MessageContext
    {
        readonly Envelope _envelope;
        Guid? _conversationId;
        Guid? _correlationId;
        Uri _destinationAddress;
        DateTime? _enqueueTime;
        DateTime? _expirationTime;
        Uri _faultAddress;
        Guid? _initiatorId;
        Guid? _messageId;
        string[] _messageType;
        Guid? _requestId;
        Uri _responseAddress;
        DateTime? _sentTime;
        Uri _sourceAddress;

        public GrpcTransportMessage(TransportMessage message, HostInfo host)
        {
            Host = host;
            Message = message;
            _envelope = message.Deliver.Envelope;

            Body = message.Deliver.Envelope.Body.ToByteArray();

            ContentType = message.Deliver.Envelope.ContentType;

            SendHeaders = new DictionarySendHeaders();

            foreach (var (key, value) in message.Deliver.Envelope.Headers)
                SendHeaders.Set(key, value);
        }

        public string[] MessageType => _messageType ??= _envelope.MessageType.ToArray();

        public string ContentType { get; }
        public byte[] Body { get; }

        public DateTime? EnqueueTime => _enqueueTime ??= _envelope.EnqueueTime.ToDateTime();

        public TransportMessage Message { get; }

        public SendHeaders SendHeaders { get; }

        public Headers Headers => SendHeaders;

        public Guid? MessageId => _messageId ??= ToGuid(_envelope.MessageId);

        public Guid? RequestId => _requestId ??= ToGuid(_envelope.RequestId);

        public Guid? CorrelationId => _correlationId ??= ToGuid(_envelope.CorrelationId);

        public Guid? ConversationId => _conversationId ??= ToGuid(_envelope.ConversationId);

        public Guid? InitiatorId => _initiatorId ??= ToGuid(_envelope.InitiatorId);

        public Uri SourceAddress => _sourceAddress ??= ToUri(_envelope.SourceAddress);

        public Uri DestinationAddress => _destinationAddress ??= ToUri(_envelope.DestinationAddress);

        public Uri ResponseAddress => _responseAddress ??= ToUri(_envelope.ResponseAddress);

        public Uri FaultAddress => _faultAddress ??= ToUri(_envelope.FaultAddress);

        public DateTime? ExpirationTime => _expirationTime ??= _envelope.ExpirationTime.ToDateTime();

        public DateTime? SentTime => _sentTime ??= _envelope.SentTime.ToDateTime();

        public HostInfo Host { get; }

        public string RoutingKey => Message.Deliver.Exchange?.RoutingKey;

        static Guid? ToGuid(string value)
        {
            return Guid.TryParse(value, out var guid)
                ? guid
                : default;
        }

        static Uri ToUri(string value)
        {
            try
            {
                return string.IsNullOrWhiteSpace(value)
                    ? default
                    : new Uri(value);
            }
            catch (FormatException)
            {
                return default;
            }
        }
    }
}
