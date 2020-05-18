namespace MassTransit.Interop.NServiceBus.Contexts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Context;
    using MassTransit.Metadata;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Serialization;


    public class NServiceBusConsumeContext :
        DeserializerConsumeContext
    {
        readonly JsonSerializer _deserializer;
        readonly JToken _messageToken;
        readonly IDictionary<Type, ConsumeContext> _messageTypes;
        readonly NServiceBusHeaderAdapter _headerAdapter;

        public NServiceBusConsumeContext(JsonSerializer deserializer, ReceiveContext receiveContext, JToken messageToken)
            : base(receiveContext)
        {
            _messageToken = messageToken ?? new JObject();

            _deserializer = deserializer;
            _messageTypes = new Dictionary<Type, ConsumeContext>();

            _headerAdapter = new NServiceBusHeaderAdapter(receiveContext.TransportHeaders);
        }

        public override Guid? MessageId => _headerAdapter.MessageId;
        public override Guid? RequestId => _headerAdapter.RequestId;
        public override Guid? CorrelationId => _headerAdapter.CorrelationId;
        public override Guid? ConversationId => _headerAdapter.ConversationId;
        public override Guid? InitiatorId => _headerAdapter.InitiatorId;
        public override DateTime? ExpirationTime => _headerAdapter.ExpirationTime;
        public override Uri SourceAddress => _headerAdapter.SourceAddress;
        public override Uri DestinationAddress => _headerAdapter.DestinationAddress;
        public override Uri ResponseAddress => _headerAdapter.ResponseAddress;
        public override Uri FaultAddress => _headerAdapter.FaultAddress;
        public override DateTime? SentTime => _headerAdapter.SentTime;
        public override Headers Headers => _headerAdapter.Headers;
        public override HostInfo Host => _headerAdapter.Host;

        public override IEnumerable<string> SupportedMessageTypes => _headerAdapter.SupportedMessageTypes;

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

                string typeUrn = MessageUrn.ForTypeString<T>();

                if (SupportedMessageTypes.Any(x => typeUrn.Equals(x, StringComparison.OrdinalIgnoreCase)))
                {
                    object obj;
                    Type deserializeType = typeof(T);
                    if (deserializeType.GetTypeInfo().IsInterface && TypeMetadataCache<T>.IsValidMessageType)
                        deserializeType = TypeMetadataCache<T>.ImplementationType;

                    using (JsonReader jsonReader = _messageToken.CreateReader())
                    {
                        obj = _deserializer.Deserialize(jsonReader, deserializeType);
                    }

                    _messageTypes[typeof(T)] = message = new MessageConsumeContext<T>(this, (T) obj);
                    return true;
                }

                _messageTypes[typeof(T)] = message = null;
                return false;
            }
        }
    }
}