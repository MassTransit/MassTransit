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


    public class RawConsumeContext :
        DeserializerConsumeContext
    {
        readonly JsonSerializer _deserializer;
        readonly RawMessageHeaderAdapter _headerAdapter;
        readonly JToken _messageToken;
        readonly IDictionary<Type, ConsumeContext> _messageTypes;
        readonly RawSerializerOptions _options;
        readonly string[] _supportedTypes;

        public RawConsumeContext(JsonSerializer deserializer, ReceiveContext receiveContext, JToken messageToken, RawSerializerOptions options)
            : base(receiveContext)
        {
            _messageToken = messageToken ?? new JObject();

            _deserializer = deserializer;
            _options = options;

            _messageTypes = new Dictionary<Type, ConsumeContext>();

            _headerAdapter = new RawMessageHeaderAdapter(receiveContext, options);

            _supportedTypes = _headerAdapter.SupportedMessageTypes;
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

        public override IEnumerable<string> SupportedMessageTypes => _supportedTypes;

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

                var typeUrn = MessageUrn.ForTypeString<T>();

                if (_options.HasFlag(RawSerializerOptions.AnyMessageType)
                    || _supportedTypes.Length == 0
                    || _supportedTypes.Any(x => typeUrn.Equals(x, StringComparison.OrdinalIgnoreCase)))
                {
                    try
                    {
                        object obj;
                        var deserializeType = typeof(T);
                        if (deserializeType.GetTypeInfo().IsInterface && TypeMetadataCache<T>.IsValidMessageType)
                            deserializeType = TypeMetadataCache<T>.ImplementationType;

                        using (var jsonReader = _messageToken.CreateReader())
                        {
                            obj = _deserializer.Deserialize(jsonReader, deserializeType);
                        }

                        _messageTypes[typeof(T)] = message = new MessageConsumeContext<T>(this, (T)obj);
                        return true;
                    }
                    catch (Exception exception)
                    {
                        LogContext.Warning?.Log(exception, "Failed to deserialize message type: {MessageType}", TypeMetadataCache<T>.ShortName);
                    }
                }

                _messageTypes[typeof(T)] = message = null;
                return false;
            }
        }
    }
}
