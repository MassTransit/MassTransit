namespace MassTransit.Serialization.JsonConverters
{
    using System;
    using System.IO;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Internals;
    using MessageData;
    using MessageData.Converters;
    using MessageData.Values;
    using Metadata;


    public class SystemTextJsonMessageDataConverter :
        JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.ClosesType(typeof(MessageData<>));
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            if (!typeToConvert.ClosesType(typeof(MessageData<>), out Type[] types))
                return null;

            var elementType = types[0];

            if (elementType == typeof(string)
                || elementType == typeof(byte[])
                || elementType == typeof(Stream))
                return (JsonConverter)Activator.CreateInstance(typeof(MessageDataConverter<>).MakeGenericType(types));

            if (TypeMetadataCache.IsValidMessageDataType(elementType))
                return (JsonConverter)Activator.CreateInstance(typeof(MessageDataObjectConverter<>).MakeGenericType(types));

            throw new MessageDataException("The message data type is not supported: " + TypeCache.GetShortName(elementType));
        }


        class MessageDataConverter<T> :
            JsonConverter<MessageData<T>>
        {
            public override MessageData<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var reference = JsonSerializer.Deserialize<SystemTextMessageDataReference>(ref reader, options);
                if (reference?.Text != null)
                    return (MessageData<T>)new StringInlineMessageData(reference.Text, reference.Reference);
                if (reference?.Data != null)
                    return (MessageData<T>)new BytesInlineMessageData(reference.Data, reference.Reference);

                if (reference?.Reference == null)
                    return EmptyMessageData<T>.Instance;

                return new DeserializedMessageData<T>(reference.Reference);
            }

            public override void Write(Utf8JsonWriter writer, MessageData<T> value, JsonSerializerOptions options)
            {
                var reference = new SystemTextMessageDataReference();

                if (value is IMessageData { HasValue: true } messageData)
                {
                    reference.Reference = messageData.Address;

                    if (messageData is IInlineMessageData inlineMessageData)
                        inlineMessageData.Set(reference);
                }

                JsonSerializer.Serialize(writer, reference, options);
            }
        }


        class MessageDataObjectConverter<T> :
            JsonConverter<MessageData<T>>
            where T : class
        {
            public override MessageData<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var converter = new SystemTextJsonObjectMessageDataConverter<T>(options);

                var reference = JsonSerializer.Deserialize<SystemTextMessageDataReference>(ref reader, options);
                if (reference?.Data != null)
                    return new BytesInlineMessageData<T>(converter, reference.Data, reference.Reference);

                if (reference?.Reference == null)
                    return EmptyMessageData<T>.Instance;

                return new DeserializedMessageData<T>(reference.Reference);
            }

            public override void Write(Utf8JsonWriter writer, MessageData<T> value, JsonSerializerOptions options)
            {
                var reference = new SystemTextMessageDataReference();

                if (value is IMessageData { HasValue: true } messageData)
                {
                    reference.Reference = messageData.Address;

                    if (messageData is IInlineMessageData inlineMessageData)
                        inlineMessageData.Set(reference);
                }

                JsonSerializer.Serialize(writer, reference, options);
            }
        }
    }
}
