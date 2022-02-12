#nullable enable
namespace MassTransit.Serialization
{
    using System;
    using System.Collections.Generic;
    using JsonConverters;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;


    public abstract class NewtonsoftSerializerContext :
        BaseSerializerContext
    {
        readonly JsonSerializer _deserializer;
        readonly object? _message;

        protected NewtonsoftSerializerContext(JsonSerializer deserializer, IObjectDeserializer objectDeserializer, MessageContext messageContext,
            object? message, string[] supportedMessageTypes)
            : base(objectDeserializer, messageContext, supportedMessageTypes)
        {
            _deserializer = deserializer;
            _message = message;
        }

        public override bool TryGetMessage<T>(out T? message)
            where T : class
        {
            var messageToken = GetMessageToken(_message);

            if (typeof(T) == typeof(JToken))
            {
                message = messageToken as T;
                return message != null;
            }

            if (IsSupportedMessageType<T>())
            {
                using var json = messageToken.CreateReader();
                message = _deserializer.Deserialize<T>(json);

                return message != null;
            }

            message = null;
            return false;
        }

        public override bool TryGetMessage(Type messageType, out object? message)
        {
            var messageToken = GetMessageToken(_message);

            using var reader = messageToken.CreateReader();
            message = _deserializer.Deserialize(reader, messageType);

            return message != null;
        }

        public override Dictionary<string, object> ToDictionary<T>(T? message)
            where T : class
        {
            if (message == null)
                return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            var dictionary = JObject.FromObject(message, NewtonsoftJsonMessageSerializer.Serializer);

            return dictionary.ToObject<Dictionary<string, object>>(_deserializer) ?? new CaseInsensitiveDictionary<object>();
        }

        static JToken GetMessageToken(object? message)
        {
            return message is JToken element
                ? element.Type == JTokenType.Null
                    ? new JObject()
                    : element
                : new JObject();
        }
    }
}
