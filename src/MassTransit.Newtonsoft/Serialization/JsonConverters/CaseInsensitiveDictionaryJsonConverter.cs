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
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
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
            if (objectType.IsGenericType)
            {
                if (objectType.ClosesType(typeof(IDictionary<,>), out Type[] elementTypes)
                    || objectType.ClosesType(typeof(IReadOnlyDictionary<,>), out elementTypes)
                    || objectType.ClosesType(typeof(Dictionary<,>), out elementTypes)
                    || (objectType.ClosesType(typeof(IEnumerable<>), out Type[] enumerableType)
                        && enumerableType[0].ClosesType(typeof(KeyValuePair<,>), out elementTypes)))
                {
                    keyType = elementTypes[0];
                    valueType = elementTypes[1];

                    if (keyType != typeof(string))
                        return false;

                    if (objectType.IsFSharpType())
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
            object IConverter.Deserialize(JsonReader reader, Type objectType, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.StartObject)
                {
                    object result = new CaseInsensitiveDictionary<T>();

                    serializer.Populate(reader, result);

                    return result;
                }

                if (reader.TokenType == JsonToken.StartArray)
                {
                    object result = new List<KeyValuePair<string, T>>();

                    serializer.Populate(reader, result);

                    return result;
                }

                return null;
            }

            public bool IsSupported => true;
        }
    }
}
