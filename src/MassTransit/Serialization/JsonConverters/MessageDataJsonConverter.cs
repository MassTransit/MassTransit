namespace MassTransit.Serialization.JsonConverters
{
    using System;
    using Internals.Extensions;
    using MessageData;
    using MessageData.Values;
    using Metadata;
    using Newtonsoft.Json;


    public class MessageDataJsonConverter :
        BaseJsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is IMessageData messageData && messageData.HasValue)
            {
                var reference = new MessageDataReference {Reference = messageData.Address};

                if (messageData is IInlineMessageData inlineMessageData)
                    inlineMessageData.Set(reference);

                serializer.Serialize(writer, reference);
            }
            else
            {
                writer.WriteNull();
            }
        }

        protected override IConverter ValueFactory(Type objectType)
        {
            if (objectType.ClosesType(typeof(MessageData<>), out Type[] dataTypes))
            {
                var elementType = dataTypes[0];
                if (elementType == typeof(string) || elementType == typeof(byte[]))
                    return (IConverter)Activator.CreateInstance(typeof(CachedConverter<>).MakeGenericType(elementType));

                throw new MessageDataException("The message data type is not supported: " + TypeMetadataCache.GetShortName(elementType));
            }

            return new Unsupported();
        }


        class CachedConverter<T> :
            IConverter
        {
            object IConverter.Deserialize(JsonReader reader, Type objectType, JsonSerializer serializer)
            {
                var reference = serializer.Deserialize<MessageDataReference>(reader);
                if (reference?.Text != null)
                    return new StringInlineMessageData(reference.Text, reference.Reference);
                if (reference?.Data != null)
                    return new BytesInlineMessageData(reference.Data, reference.Reference);

                if (reference?.Reference == null)
                    return EmptyMessageData<T>.Instance;

                return new DeserializedMessageData<T>(reference.Reference);
            }

            public bool IsSupported => true;
        }
    }
}
