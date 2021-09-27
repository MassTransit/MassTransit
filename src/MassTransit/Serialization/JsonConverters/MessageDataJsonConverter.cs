namespace MassTransit.Serialization.JsonConverters
{
    using System;
    using System.IO;
    using Internals.Extensions;
    using MessageData;
    using MessageData.PropertyProviders;
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
                var reference = new MessageDataReference { Reference = messageData.Address };

                if (messageData is IInlineMessageData inlineMessageData)
                    inlineMessageData.Set(reference);

                serializer.Serialize(writer, reference);
            }
            else
                writer.WriteNull();
        }

        protected override IConverter ValueFactory(Type objectType)
        {
            if (objectType.ClosesType(typeof(MessageData<>), out Type[] dataTypes))
            {
                var elementType = dataTypes[0];
                if (elementType == typeof(string)
                    || elementType == typeof(byte[])
                    || elementType == typeof(Stream))
                    return (IConverter)Activator.CreateInstance(typeof(CachedConverter<>).MakeGenericType(elementType));

                if (MessageDataExtensions.IsValidMessageDataType(elementType))
                    return (IConverter)Activator.CreateInstance(typeof(ObjectConverter<>).MakeGenericType(elementType));

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


        class ObjectConverter<T> :
            IConverter
        {
            readonly IMessageDataConverter<T> _converter;

            public ObjectConverter()
            {
                _converter = new ObjectMessageDataConverter<T>();
            }

            object IConverter.Deserialize(JsonReader reader, Type objectType, JsonSerializer serializer)
            {
                var reference = serializer.Deserialize<MessageDataReference>(reader);
                if (reference?.Data != null)
                    return new BytesInlineMessageData<T>(_converter, reference.Data, reference.Reference);

                if (reference?.Reference == null)
                    return EmptyMessageData<T>.Instance;

                return new DeserializedMessageData<T>(reference.Reference);
            }

            public bool IsSupported => true;
        }
    }
}
