namespace MassTransit.KafkaIntegration
{
    using System;
    using Confluent.Kafka;


    public class KafkaHeaderAdapter<TKey, TValue> :
        MessageContext
    {
        readonly ReceiveContext _receiveContext;
        readonly ConsumeResult<TKey, TValue> _result;
        Guid? _conversationId;
        Guid? _correlationId;
        Guid? _initiatorId;
        Guid? _messageId;
        DateTime? _sentTime;
        Uri _sourceAddress;

        public KafkaHeaderAdapter(ConsumeResult<TKey, TValue> result, ReceiveContext receiveContext)
        {
            _result = result;
            _receiveContext = receiveContext;
        }

        public Guid? MessageId => _messageId ??= Headers.Get<Guid>(nameof(MessageId));
        public Guid? RequestId { get; } = default;
        public Guid? CorrelationId => _correlationId ??= Headers.Get<Guid>(nameof(CorrelationId));
        public Guid? ConversationId => _conversationId ??= Headers.Get<Guid>(nameof(ConversationId));
        public Guid? InitiatorId => _initiatorId ??= Headers.Get<Guid>(nameof(InitiatorId));
        public DateTime? ExpirationTime { get; } = default;
        public Uri SourceAddress => _sourceAddress ??= GetEndpointAddress(nameof(SourceAddress));
        public Uri DestinationAddress => _receiveContext.InputAddress;
        public Uri ResponseAddress { get; } = default;
        public Uri FaultAddress { get; } = default;
        public DateTime? SentTime => _sentTime ??= _result.Message.Timestamp.UtcDateTime;
        public MassTransit.Headers Headers => _receiveContext.TransportHeaders;
        public HostInfo Host => default;

        Uri GetEndpointAddress(string key)
        {
            var endpoint = Headers.Get<string>(key);
            return string.IsNullOrWhiteSpace(endpoint)
                ? default
                : new Uri(endpoint);
        }
    }
}
