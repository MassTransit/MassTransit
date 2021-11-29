namespace MassTransit.Serialization.JsonConverters
{
    using System;
    using System.Reflection;
    using Metadata;
    using Newtonsoft.Json;


    public class InterfaceProxyConverter :
        BaseJsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        protected override IConverter ValueFactory(Type objectType)
        {
            if (objectType.GetTypeInfo().IsInterface && MessageTypeCache.IsValidMessageType(objectType))
                return (IConverter)Activator.CreateInstance(typeof(CachedConverter<>).MakeGenericType(objectType));

            return new Unsupported();
        }


        class CachedConverter<T> :
            IConverter
        {
            object IConverter.Deserialize(JsonReader reader, Type objectType, JsonSerializer serializer)
            {
                return serializer.Deserialize(reader, TypeMetadataCache<T>.ImplementationType);
            }

            public bool IsSupported => true;
        }
    }
}
