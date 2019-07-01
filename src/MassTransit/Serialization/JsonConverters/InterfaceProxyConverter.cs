namespace MassTransit.Serialization.JsonConverters
{
    using System;
    using System.Reflection;
    using Newtonsoft.Json;
    using Util;


    public class InterfaceProxyConverter :
        BaseJsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        protected override IConverter ValueFactory(Type type)
        {
            if (type.GetTypeInfo().IsInterface && TypeMetadataCache.IsValidMessageType(type))
                return (IConverter)Activator.CreateInstance(typeof(CachedConverter<>).MakeGenericType(type));

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
