#nullable enable
namespace MassTransit.Serialization
{
    using System;
    using System.Linq;
    using System.Net.Mime;
    using System.Text.Json;


    public class SystemTextJsonRawSerializerContext :
        SystemTextJsonSerializerContext
    {
        readonly RawSerializerOptions _rawOptions;

        public SystemTextJsonRawSerializerContext(IObjectDeserializer objectDeserializer, JsonSerializerOptions options, ContentType contentType,
            MessageContext messageContext, string[] messageTypes, RawSerializerOptions rawOptions, JsonElement message)
            : base(objectDeserializer, options, contentType, messageContext, messageTypes, message: message)
        {
            _rawOptions = rawOptions;
        }

        public override IMessageSerializer GetMessageSerializer()
        {
            return new SystemTextJsonBodyMessageSerializer(Message, ContentType, Options, _rawOptions);
        }

        public override bool IsSupportedMessageType<T>()
        {
            var typeUrn = MessageUrn.ForTypeString<T>();

            return _rawOptions.HasFlag(RawSerializerOptions.AnyMessageType)
                || SupportedMessageTypes.Length == 0
                || SupportedMessageTypes.Any(x => typeUrn.Equals(x, StringComparison.OrdinalIgnoreCase));
        }

        public override bool IsSupportedMessageType(Type messageType)
        {
            var typeUrn = MessageUrn.ForTypeString(messageType);

            return _rawOptions.HasFlag(RawSerializerOptions.AnyMessageType)
                || SupportedMessageTypes.Length == 0
                || SupportedMessageTypes.Any(x => typeUrn.Equals(x, StringComparison.OrdinalIgnoreCase));
        }

        public override IMessageSerializer GetMessageSerializer(object message, string[] messageTypes)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            return new SystemTextJsonBodyMessageSerializer(message, ContentType, Options, _rawOptions);
        }

        public override IMessageSerializer GetMessageSerializer<T>(MessageEnvelope envelope, T message)
        {
            var serializer = new SystemTextJsonBodyMessageSerializer(envelope, ContentType, Options, _rawOptions);

            serializer.Overlay(message);

            return serializer;
        }
    }
}
