namespace MassTransit.Serialization
{
    using System;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;


    public class NewtonsoftRawBsonSerializerContext :
        NewtonsoftSerializerContext
    {
        readonly ContentType _contentType;
        readonly JToken _message;

        public NewtonsoftRawBsonSerializerContext(JsonSerializer deserializer, IObjectDeserializer objectDeserializer, MessageContext context, JToken message,
            string[] supportedMessageTypes, ContentType contentType)
            : base(deserializer, objectDeserializer, context, message, supportedMessageTypes)
        {
            _contentType = contentType;
            _message = message;
        }

        public override IMessageSerializer GetMessageSerializer()
        {
            return new NewtonsoftRawBsonBodyMessageSerializer(_message, _contentType);
        }

        public override IMessageSerializer GetMessageSerializer<T>(MessageEnvelope envelope, T message)
        {
            var serializer = new NewtonsoftRawBsonBodyMessageSerializer(_message, _contentType);

            serializer.Overlay(message);

            return serializer;
        }

        public override IMessageSerializer GetMessageSerializer(object message, string[] messageTypes)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (message is JToken messageToken)
                return new NewtonsoftRawBsonBodyMessageSerializer(messageToken, _contentType, messageTypes);

            throw new SerializationException($"Unable to create message serializer for object: {TypeCache.GetShortName(message.GetType())}");
        }
    }
}
