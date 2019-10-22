namespace MassTransit.Serialization.JsonConverters
{
    using System;
    using Internals.Extensions;
    using MessageData;
    using Metadata;
    using Newtonsoft.Json;
    using Util;


    public class MessageDataJsonConverter :
        BaseJsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var messageData = value as IMessageData;
            if (messageData == null)
                return;

            var reference = new MessageDataReference {Reference = messageData.Address};

            serializer.Serialize(writer, reference);
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
                if (reference?.Reference == null)
                    return new EmptyMessageData<T>();

                return new DeserializedMessageData<T>(reference.Reference);
            }

            public bool IsSupported => true;
        }
    }
}
