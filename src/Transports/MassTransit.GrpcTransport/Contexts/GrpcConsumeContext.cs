namespace MassTransit.GrpcTransport.Contexts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Context;
    using Metadata;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;


    public class GrpcConsumeContext :
        DeserializerConsumeContext
    {
        readonly GrpcReceiveContext _context;
        readonly JsonSerializer _deserializer;
        readonly JToken _messageToken;
        readonly IDictionary<Type, ConsumeContext> _messageTypes;

        public GrpcConsumeContext(JsonSerializer deserializer, ReceiveContext receiveContext, JToken messageToken)
            : base(receiveContext)
        {
            _messageToken = messageToken ?? new JObject();
            _deserializer = deserializer;

            _context = receiveContext as GrpcReceiveContext ?? throw new ArgumentException("Must be GrpcReceiveContext", nameof(receiveContext));

            _messageTypes = new Dictionary<Type, ConsumeContext>();
        }

        public override Guid? MessageId => _context.Message.MessageId;

        public override Guid? RequestId => _context.Message.RequestId;

        public override Guid? CorrelationId => _context.Message.CorrelationId;

        public override Guid? ConversationId => _context.Message.ConversationId;

        public override Guid? InitiatorId => _context.Message.InitiatorId;

        public override DateTime? ExpirationTime => _context.Message.ExpirationTime;

        public override Uri SourceAddress => _context.Message.SourceAddress;

        public override Uri DestinationAddress => _context.Message.DestinationAddress;

        public override Uri ResponseAddress => _context.Message.ResponseAddress;

        public override Uri FaultAddress => _context.Message.FaultAddress;

        public override DateTime? SentTime => _context.Message.SentTime;

        public override Headers Headers => _context.Message.Headers;

        public override HostInfo Host => _context.Message.Host;

        public override IEnumerable<string> SupportedMessageTypes => _context.Message.MessageType;

        public override bool HasMessageType(Type messageType)
        {
            lock (_messageTypes)
            {
                if (_messageTypes.TryGetValue(messageType, out var existing))
                    return existing != null;
            }

            var typeUrn = MessageUrn.ForTypeString(messageType);

            return _context.Message.MessageType.Any(x => typeUrn.Equals(x, StringComparison.OrdinalIgnoreCase));
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

                if (_context.Message.MessageType.Any(x => typeUrn.Equals(x, StringComparison.OrdinalIgnoreCase)))
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
