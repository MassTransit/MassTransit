#nullable enable
namespace MassTransit.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Context;


    public class BodyConsumeContext :
        DeserializerConsumeContext
    {
        readonly IDictionary<Type, ConsumeContext?> _messageTypes;

        public BodyConsumeContext(ReceiveContext receiveContext, SerializerContext serializerContext)
            : base(receiveContext, serializerContext)
        {
            _messageTypes = new Dictionary<Type, ConsumeContext?>(1);
        }

        public override Guid? MessageId => SerializerContext.MessageId;
        public override Guid? RequestId => SerializerContext.RequestId;
        public override Guid? CorrelationId => SerializerContext.CorrelationId;
        public override Guid? ConversationId => SerializerContext.ConversationId;
        public override Guid? InitiatorId => SerializerContext.InitiatorId;
        public override DateTime? ExpirationTime => SerializerContext.ExpirationTime;
        public override Uri SourceAddress => SerializerContext.SourceAddress!;
        public override Uri DestinationAddress => SerializerContext.DestinationAddress!;
        public override Uri ResponseAddress => SerializerContext.ResponseAddress!;
        public override Uri FaultAddress => SerializerContext.FaultAddress!;
        public override DateTime? SentTime => SerializerContext.SentTime;
        public override Headers Headers => SerializerContext.Headers;
        public override HostInfo Host => SerializerContext.Host;
        public override IEnumerable<string> SupportedMessageTypes => SerializerContext.SupportedMessageTypes;

        public override bool HasMessageType(Type messageType)
        {
            lock (_messageTypes)
            {
                if (_messageTypes.TryGetValue(messageType, out var existing))
                    return existing != null;
            }

            return SerializerContext.IsSupportedMessageType(messageType);
        }

        public override bool TryGetMessage<T>([NotNullWhen(true)] out ConsumeContext<T>? message)
        {
            lock (_messageTypes)
            {
                if (_messageTypes.TryGetValue(typeof(T), out var existing))
                {
                    message = (existing as ConsumeContext<T>)!;
                    return message != null;
                }

                if (typeof(T).IsInterface && MessageTypeCache<T>.IsValidMessageType)
                {
                    if (SerializerContext.IsSupportedMessageType<T>())
                    {
                        if (SerializerContext.TryGetMessage(typeof(T), out var messageObj))
                        {
                            _messageTypes[typeof(T)] = message = new MessageConsumeContext<T>(this, (T)messageObj);
                            return true;
                        }
                    }
                }

                if (SerializerContext.TryGetMessage<T>(out var messageOfT))
                {
                    _messageTypes[typeof(T)] = message = new MessageConsumeContext<T>(this, messageOfT!);
                    return true;
                }

                _messageTypes[typeof(T)] = message = null;
                return false;
            }
        }
    }
}
