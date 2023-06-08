#nullable enable
namespace MassTransit.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Net.Mime;
    using ProtoBuf;
    using ProtoBuf.Meta;
    using MassTransit;
    using System.IO;

    public abstract class ProtobufBodySerializerContext :
        BaseSerializerContext
    {
        readonly RuntimeTypeModel _typeModel;
        private object? _message;

        protected ProtobufBodySerializerContext(RuntimeTypeModel typeModel, IObjectDeserializer objectDeserializer, MessageContext messageContext,
            object? message, string[] supportedMessageTypes)
            : base(objectDeserializer, messageContext, supportedMessageTypes)
        {
            _typeModel = typeModel;
            _message = message;
        }

        public override bool TryGetMessage<T>(out T? message)
            where T : class
        {
            if (_typeModel.CanSerialize(typeof(T)))
            {
                if (_message is T messageOfT)
                {
                    message = messageOfT;
                    return true;
                }

                using (var stream = new System.IO.MemoryStream())
                {
                    _typeModel.Serialize(stream, _message);
                    stream.Position = 0;

                    message = (T)_typeModel.Deserialize(stream, null, typeof(T));
                    return true;
                }
            }

            message = null;
            return false;
        }

        public override bool TryGetMessage(Type messageType, [NotNullWhen(true)] out object? message)
        {
            if (_message != null && messageType.IsInstanceOfType(_message))
            {
                message = _message;
                return true;
            }

            if (_typeModel.CanSerialize(messageType))
            {
                using (var stream = new System.IO.MemoryStream())
                {
                    _typeModel.Serialize(stream, _message);
                    stream.Position = 0;

                    message = _typeModel.Deserialize(stream, null, messageType);
                    return true;
                }
            }

            message = null;
            return false;
        }

        public override Dictionary<string, object> ToDictionary<T>(T? message)
            where T : class
        {
            if (message == null)
                return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            using var stream = new MemoryStream();
            _typeModel.Serialize(stream, message);
            stream.Position = 0;
            return Serializer.Merge(stream, new Dictionary<string, object>())!;
        }
    }

#nullable enable
    public class ProtobufSerializerContext :
        ProtobufBodySerializerContext
    {
        readonly ContentType _contentType;
        readonly MessageEnvelope _envelope;
        readonly RuntimeTypeModel _typeModel;

        public ProtobufSerializerContext(RuntimeTypeModel typeModel, IObjectDeserializer objectDeserializer, MessageEnvelope envelope,
            ContentType contentType)
            : base(typeModel, objectDeserializer, new EnvelopeMessageContext(envelope, objectDeserializer), envelope.Message,
                envelope.MessageType ?? Array.Empty<string>())
        {
            _contentType = contentType;
            _envelope = envelope;
            _typeModel = typeModel;
        }

        public override IMessageSerializer GetMessageSerializer()
        {
            return new ProtobufBodyMessageSerializer(_envelope, _contentType, _typeModel);
        }

        public override IMessageSerializer GetMessageSerializer<T>(MessageEnvelope envelope, T message)
        {
            var serializer = new ProtobufBodyMessageSerializer(envelope, _contentType, _typeModel);

            serializer.Overlay(message);

            return serializer;
        }

        public override IMessageSerializer GetMessageSerializer(object message, string[] messageTypes)
        {
            var envelope = new ProtobufMessageEnvelope<object>(this, message, messageTypes);

            return new ProtobufBodyMessageSerializer((MessageEnvelope)envelope, _contentType, _typeModel);
        }
    }
}
