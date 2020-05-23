namespace MassTransit.KafkaIntegration.Contexts
{
    using System;
    using Confluent.Kafka;
    using Metadata;


    public class KafkaHeaderAdapter<TKey, TValue> :
        MessageContext
    {
        readonly ConsumeResult<TKey, TValue> _consumeResult;
        readonly ReceiveContext _receiveContext;
        Guid? _conversationId;
        Guid? _correlationId;
        HostInfo _host;
        Guid? _messageId;
        DateTime? _sentTime;
        Uri _sourceAddress;

        public KafkaHeaderAdapter(ConsumeResult<TKey, TValue> consumeResult, ReceiveContext receiveContext)
        {
            _consumeResult = consumeResult;
            _receiveContext = receiveContext;
        }

        public Guid? MessageId => _messageId ??= Headers.Get<Guid>(MessageHeaders.MessageId);
        public Guid? RequestId { get; } = default;
        public Guid? CorrelationId => _correlationId ??= Headers.Get<Guid>("MT-CorrelationId");
        public Guid? ConversationId => _conversationId ??= Headers.Get<Guid>("MT-ConversationId");
        public Guid? InitiatorId { get; } = default;
        public DateTime? ExpirationTime { get; } = default;
        public Uri SourceAddress => _sourceAddress ??= GetEndpointAddress("MT-Source-Address");
        public Uri DestinationAddress => _receiveContext.InputAddress;
        public Uri ResponseAddress { get; } = default;
        public Uri FaultAddress { get; } = default;
        public DateTime? SentTime => _sentTime ??= _consumeResult.Message.Timestamp.UtcDateTime;
        public MassTransit.Headers Headers => _receiveContext.TransportHeaders;
        public HostInfo Host => _host ??= HostMetadataCache.Host;

        Uri GetEndpointAddress(string key)
        {
            var endpoint = Headers.Get<string>(key);
            return string.IsNullOrWhiteSpace(endpoint)
                ? default
                : new Uri($"queue:{endpoint}");
        }
    }
}
