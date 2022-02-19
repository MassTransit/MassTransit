#nullable enable
namespace MassTransit.GrpcTransport.Fabric
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Contracts;
    using Serialization;


    public class GrpcTransportMessage :
        MessageContext,
        Headers,
        GrpcConsumeContext
    {
        readonly Envelope _envelope;
        Guid? _conversationId;
        Guid? _correlationId;
        Uri? _destinationAddress;
        DateTime? _enqueueTime;
        DateTime? _expirationTime;
        Uri? _faultAddress;
        Guid? _initiatorId;
        Guid? _messageId;
        string[]? _messageType;
        Guid? _requestId;
        Uri? _responseAddress;
        DateTime? _sentTime;
        Uri? _sourceAddress;

        public GrpcTransportMessage(TransportMessage message, HostInfo host)
        {
            Host = host;
            Message = message;
            _envelope = message.Deliver.Envelope;

            Body = message.Deliver.Envelope.Body.ToByteArray();

            ContentType = message.Deliver.Envelope.ContentType;

            SendHeaders = new DictionarySendHeaders();

            foreach (KeyValuePair<string, string> header in message.Deliver.Envelope.Headers)
                SendHeaders.Set(header.Key, header.Value);
        }

        public string[] MessageType => _messageType ??= _envelope.MessageType.ToArray();

        public string ContentType { get; }
        public byte[] Body { get; }

        public DateTime? EnqueueTime => _enqueueTime ??= _envelope.EnqueueTime.ToDateTime();

        public TransportMessage Message { get; }

        public SendHeaders SendHeaders { get; }

        public string? RoutingKey => Message.Deliver.Exchange?.RoutingKey;

        public IEnumerator<HeaderValue> GetEnumerator()
        {
            return Headers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<KeyValuePair<string, object>> GetAll()
        {
            return Headers.GetAll();
        }

        public bool TryGetHeader(string key, out object? value)
        {
            switch (key)
            {
                case MessageHeaders.MessageId:
                    value = MessageId;
                    return true;
                case MessageHeaders.CorrelationId:
                    value = CorrelationId;
                    return true;
                case MessageHeaders.ConversationId:
                    value = ConversationId;
                    return true;
                case MessageHeaders.RequestId:
                    value = RequestId;
                    return true;
                case MessageHeaders.InitiatorId:
                    value = InitiatorId;
                    return true;
                case MessageHeaders.SourceAddress:
                    value = SourceAddress;
                    return true;
                case MessageHeaders.ResponseAddress:
                    value = ResponseAddress;
                    return true;
                case MessageHeaders.FaultAddress:
                    value = FaultAddress;
                    return true;
            }

            return Headers.TryGetHeader(key, out value);
        }

        public T? Get<T>(string key, T? defaultValue = default)
            where T : class
        {
            return TryGetHeader(key, out var value) ? SystemTextJsonMessageSerializer.Instance.DeserializeObject(value, defaultValue) : default;
        }

        public T? Get<T>(string key, T? defaultValue = default)
            where T : struct
        {
            return TryGetHeader(key, out var value) ? SystemTextJsonMessageSerializer.Instance.DeserializeObject(value, defaultValue) : default;
        }

        public Headers Headers => SendHeaders;

        public Guid? MessageId => _messageId ??= ToGuid(_envelope.MessageId);

        public Guid? RequestId => _requestId ??= ToGuid(_envelope.RequestId);

        public Guid? CorrelationId => _correlationId ??= ToGuid(_envelope.CorrelationId);

        public Guid? ConversationId => _conversationId ??= ToGuid(_envelope.ConversationId);

        public Guid? InitiatorId => _initiatorId ??= ToGuid(_envelope.InitiatorId);

        public Uri? SourceAddress => _sourceAddress ??= ToUri(_envelope.SourceAddress);

        public Uri? DestinationAddress => _destinationAddress ??= ToUri(_envelope.DestinationAddress);

        public Uri? ResponseAddress => _responseAddress ??= ToUri(_envelope.ResponseAddress);

        public Uri? FaultAddress => _faultAddress ??= ToUri(_envelope.FaultAddress);

        public DateTime? ExpirationTime => _expirationTime ??= _envelope.ExpirationTime.ToDateTime();

        public DateTime? SentTime => _sentTime ??= _envelope.SentTime.ToDateTime();

        public HostInfo Host { get; }

        static Guid? ToGuid(string? value)
        {
            return Guid.TryParse(value, out var guid)
                ? guid
                : default;
        }

        static Uri? ToUri(string? value)
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
