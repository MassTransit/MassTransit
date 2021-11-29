namespace MassTransit.Serialization
{
    using System;
    using System.Linq;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;


    public class NewtonsoftRawJsonSerializerContext :
        NewtonsoftSerializerContext
    {
        readonly ContentType _contentType;
        readonly JToken _message;
        readonly RawSerializerOptions _options;

        public NewtonsoftRawJsonSerializerContext(JsonSerializer deserializer, IObjectDeserializer objectDeserializer, MessageContext context, JToken message,
            string[] supportedMessageTypes, RawSerializerOptions options, ContentType contentType)
            : base(deserializer, objectDeserializer, context, message, supportedMessageTypes)
        {
            _contentType = contentType;
            _message = message;
            _options = options;
        }

        public override bool IsSupportedMessageType<T>()
        {
            var typeUrn = MessageUrn.ForTypeString<T>();

            return _options.HasFlag(RawSerializerOptions.AnyMessageType)
                || SupportedMessageTypes.Length == 0
                || SupportedMessageTypes.Any(x => typeUrn.Equals(x, StringComparison.OrdinalIgnoreCase));
        }

        public override IMessageSerializer GetMessageSerializer()
        {
            return new NewtonsoftRawJsonBodyMessageSerializer(_message, _contentType, _options);
        }

        public override IMessageSerializer GetMessageSerializer<T>(MessageEnvelope envelope, T message)
        {
            var serializer = new NewtonsoftJsonBodyMessageSerializer(envelope, _contentType);

            serializer.Overlay(message);

            return serializer;
        }

        public override IMessageSerializer GetMessageSerializer(object message, string[] messageTypes)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (message is JToken messageToken)
                return new NewtonsoftRawJsonBodyMessageSerializer(messageToken, _contentType, _options, messageTypes);

            throw new SerializationException($"Unable to create message serializer for object: {TypeCache.GetShortName(message.GetType())}");
        }
    }
}
