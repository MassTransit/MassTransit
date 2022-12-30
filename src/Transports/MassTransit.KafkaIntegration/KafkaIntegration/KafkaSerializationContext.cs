namespace MassTransit.KafkaIntegration
{
    using System;
    using System.Collections.Generic;
    using Confluent.Kafka;
    using Serialization;
    using Util;


    public class KafkaSerializationContext<TMessage> :
        BaseSerializerContext
        where TMessage : class
    {
        readonly Lazy<TMessage> _message;

        public KafkaSerializationContext(ConsumeResult<byte[], byte[]> result, IDeserializer<TMessage> deserializer, MessageContext messageContext)
            : base(SystemTextJsonMessageSerializer.Instance, messageContext, MessageTypeCache<TMessage>.MessageTypeNames)
        {
            _message = new Lazy<TMessage>(() => deserializer.DeserializeValue(result));
        }

        public override bool IsSupportedMessageType<T>()
        {
            return typeof(T).IsAssignableFrom(typeof(TMessage));
        }

        public override bool IsSupportedMessageType(Type messageType)
        {
            return messageType.IsAssignableFrom(typeof(TMessage));
        }

        public override bool TryGetMessage<T>(out T message)
            where T : class
        {
            if (_message.Value is T result)
            {
                message = result;
                return true;
            }

            message = default;
            return false;
        }

        public override bool TryGetMessage(Type messageType, out object message)
        {
            if (messageType.IsAssignableFrom(typeof(TMessage)))
            {
                message = _message.Value;
                return true;
            }

            message = null;
            return false;
        }

        public override IMessageSerializer GetMessageSerializer()
        {
            throw new NotImplementedByDesignException();
        }

        public override IMessageSerializer GetMessageSerializer<T>(MessageEnvelope envelope, T message)
        {
            var serializer = new SystemTextJsonBodyMessageSerializer(envelope, SystemTextJsonMessageSerializer.JsonContentType,
                SystemTextJsonMessageSerializer.Options);

            serializer.Overlay(message);

            return serializer;
        }

        public override IMessageSerializer GetMessageSerializer(object message, string[] messageTypes)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var envelope = new JsonMessageEnvelope(this, message, messageTypes);

            var serializer = new SystemTextJsonBodyMessageSerializer(envelope, SystemTextJsonMessageSerializer.JsonContentType,
                SystemTextJsonMessageSerializer.Options);

            return serializer;
        }

        public override Dictionary<string, object> ToDictionary<T>(T message)
            where T : class
        {
            return ConvertObject.ToDictionary(message);
        }
    }
}
