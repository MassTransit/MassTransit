namespace MassTransit.Serialization.JsonConverters
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Internals;
    using Newtonsoft.Json;


    public class CaseInsensitiveDictionaryJsonConverter :
        BaseJsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            throw new NotSupportedException("This converter should not be used for writing as it can create loops");
        }

        protected override IConverter ValueFactory(Type objectType)
        {
            if (CanConvert(objectType, out _, out var valueType) && valueType == typeof(object))
                return (IConverter)Activator.CreateInstance(typeof(CachedConverter<>).MakeGenericType(valueType));

            return new Unsupported();
        }

        static bool CanConvert(Type objectType, out Type keyType, out Type valueType)
        {
            var typeInfo = objectType.GetTypeInfo();
            if (typeInfo.IsGenericType)
            {
                if (typeInfo.ClosesType(typeof(IDictionary<,>), out Type[] elementTypes)
                    || typeInfo.ClosesType(typeof(IReadOnlyDictionary<,>), out elementTypes)
                    || typeInfo.ClosesType(typeof(Dictionary<,>), out elementTypes)
                    || typeInfo.ClosesType(typeof(IEnumerable<>), out Type[] enumerableType)
                    && enumerableType[0].ClosesType(typeof(KeyValuePair<,>), out elementTypes))
                {
                    keyType = elementTypes[0];
                    valueType = elementTypes[1];

                    if (keyType != typeof(string))
                        return false;

                    if (typeInfo.IsFSharpType())
                        return false;

                    return true;
                }
            }

            keyType = default;
            valueType = default;
            return false;
        }


        class CachedConverter<T> :
            IConverter
        {
            object IConverter.Deserialize(JsonReader reader, Type objectType, Newtonsoft.Json.JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.StartObject)
                {
                    object result = new CaseInsensitiveDictionary<T>();

                    serializer.Populate(reader, result);

                    return result;
                }

                return null;
            }

            public bool IsSupported => true;
        }
    }
}
