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

        public Guid? MessageId => _messageId ??= _envelope.MessageId.ToGuid();

        public Guid? RequestId => _requestId ??= _envelope.RequestId.ToGuid();

        public Guid? CorrelationId => _correlationId ??= _envelope.CorrelationId.ToGuid();

        public Guid? ConversationId => _conversationId ??= _envelope.ConversationId.ToGuid();

        public Guid? InitiatorId => _initiatorId ??= _envelope.InitiatorId.ToGuid();

        public Uri SourceAddress => _sourceAddress ??= _envelope.SourceAddress.ToUri();

        public Uri DestinationAddress => _destinationAddress ??= _envelope.DestinationAddress.ToUri();

        public Uri ResponseAddress => _responseAddress ??= _envelope.ResponseAddress.ToUri();

        public Uri FaultAddress => _faultAddress ??= _envelope.FaultAddress.ToUri();

        public DateTime? ExpirationTime => _expirationTime ??= _envelope.ExpirationTime.ToDateTime();

        public DateTime? SentTime => _sentTime ??= _envelope.SentTime.ToDateTime();

        public HostInfo Host { get; }

        public string RoutingKey => Message.Deliver.Exchange?.RoutingKey?.Value;
    }
}
