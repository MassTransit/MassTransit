namespace MassTransit.Serialization.JsonConverters
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Internals.Extensions;
    using MessageData;
    using MessageData.Values;


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

            return (JsonConverter)Activator.CreateInstance(typeof(MessageDataConverter<>).MakeGenericType(types));
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
    }
}
