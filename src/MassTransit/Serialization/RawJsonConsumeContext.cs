namespace MassTransit.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Context;
    using Metadata;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;


    public class RawJsonConsumeContext :
        DeserializerConsumeContext
    {
        readonly JsonSerializer _deserializer;
        readonly JToken _messageToken;
        readonly IDictionary<Type, ConsumeContext> _messageTypes;

        public RawJsonConsumeContext(JsonSerializer deserializer, ReceiveContext receiveContext, JToken messageToken)
            : base(receiveContext)
        {
            _messageToken = messageToken ?? new JObject();

            _deserializer = deserializer;
            _messageTypes = new Dictionary<Type, ConsumeContext>();

            MessageId = receiveContext.TransportHeaders.Get<Guid>(nameof(MessageContext.MessageId));
            CorrelationId = receiveContext.TransportHeaders.Get<Guid>(nameof(MessageContext.CorrelationId));
            RequestId = receiveContext.TransportHeaders.Get<Guid>(nameof(MessageContext.RequestId));
        }

        public override Guid? MessageId { get; }

        public override Guid? RequestId { get; }

        public override Guid? CorrelationId { get; }

        public override Guid? ConversationId { get; } = default;

        public override Guid? InitiatorId { get; } = default;

        public override DateTime? ExpirationTime => default;

        public override Uri SourceAddress { get; } = default;

        public override Uri DestinationAddress { get; } = default;

        public override Uri ResponseAddress { get; } = default;

        public override Uri FaultAddress { get; } = default;

        public override DateTime? SentTime => default;

        public override Headers Headers => NoMessageHeaders.Instance;

        public override HostInfo Host => default;
        public override IEnumerable<string> SupportedMessageTypes => Enumerable.Empty<string>();

        public override bool HasMessageType(Type messageType)
        {
            lock (_messageTypes)
            {
                if (_messageTypes.TryGetValue(messageType, out var existing))
                    return existing != null;
            }

            return false;
        }

        public override bool TryGetMessage<T>(out ConsumeContext<T> message)
        {
            lock (_messageTypes)
            {
                if (_messageTypes.TryGetValue(typeof(T), out var existing))
                {
                    message = existing as ConsumeContext<T>;
                    return message != null;
                }

                if (typeof(T) == typeof(JToken))
                {
                    _messageTypes[typeof(T)] = message = new MessageConsumeContext<T>(this, _messageToken as T);
                    return true;
                }

                try
                {
                    object obj;
                    Type deserializeType = typeof(T);
                    if (deserializeType.GetTypeInfo().IsInterface && TypeMetadataCache<T>.IsValidMessageType)
                        deserializeType = TypeMetadataCache<T>.ImplementationType;

                    using (JsonReader jsonReader = _messageToken.CreateReader())
                    {
                        obj = _deserializer.Deserialize(jsonReader, deserializeType);
                    }

                    _messageTypes[typeof(T)] = message = new MessageConsumeContext<T>(this, (T)obj);
                    return true;
                }
                catch (Exception)
                {
                    _messageTypes[typeof(T)] = message = null;
                    return false;
                }
            }
        }
    }
}
