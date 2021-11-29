namespace MassTransit.Serialization.JsonConverters
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Internals;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;


    public class ListJsonConverter :
        BaseJsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotSupportedException("This converter should not be used for writing as it can create loops");
        }

        protected override IConverter ValueFactory(Type objectType)
        {
            if (CanConvert(objectType, out var elementType))
                return (IConverter)Activator.CreateInstance(typeof(CachedConverter<>).MakeGenericType(elementType));

            return new Unsupported();
        }

        static bool CanConvert(Type objectType, out Type elementType)
        {
            var typeInfo = objectType.GetTypeInfo();
            if (typeInfo.IsGenericType)
            {
                if (typeInfo.ClosesType(typeof(IDictionary<,>))
                    || typeInfo.ClosesType(typeof(IReadOnlyDictionary<,>))
                    || typeInfo.ClosesType(typeof(Dictionary<,>))
                    || typeInfo.ClosesType(typeof(IEnumerable<>), out Type[] enumerableType) && enumerableType[0].ClosesType(typeof(KeyValuePair<,>)))
                {
                    elementType = default;
                    return false;
                }

                if (typeInfo.ClosesType(typeof(IList<>), out Type[] elementTypes)
                    || typeInfo.ClosesType(typeof(IReadOnlyList<>), out elementTypes)
                    || typeInfo.ClosesType(typeof(List<>), out elementTypes)
                    || typeInfo.ClosesType(typeof(IReadOnlyCollection<>), out elementTypes)
                    || typeInfo.ClosesType(typeof(IEnumerable<>), out elementTypes))
                {
                    elementType = elementTypes[0];
                    if (elementType.IsAbstract)
                        return false;

                    if (typeInfo.IsFSharpType())
                        return false;

                    return true;
                }
            }

            if (typeInfo.IsArray && typeInfo.HasElementType && typeInfo.GetArrayRank() == 1)
            {
                elementType = typeInfo.GetElementType();
                if (elementType == typeof(byte))
                    return false;

                if (elementType.IsAbstract)
                    return false;

                return objectType.HasInterface<IEnumerable>();
            }

            elementType = default;
            return false;
        }


        class CachedConverter<T> :
            IConverter
        {
            JsonArrayContract _contract;

            object IConverter.Deserialize(JsonReader reader, Type objectType, JsonSerializer serializer)
            {
                var contract = _contract ??= ResolveContract(objectType, serializer);

                if (reader.TokenType == JsonToken.Null)
                    return null;

                object result;
                if (reader.TokenType == JsonToken.StartArray)
                {
                    result = contract.DefaultCreator != null
                        ? contract.DefaultCreator()
                        : new List<T>();

                    serializer.Populate(reader, result);
                }
                else
                {
                    var item = (T)serializer.Deserialize(reader, typeof(T));
                    result = new List<T> {item};
                }

                if (contract.CreatedType.IsArray && result is IEnumerable<T> enumerable)
                    return enumerable.ToArray();

                return result;
            }

            public bool IsSupported => true;

            static JsonArrayContract ResolveContract(Type objectType, JsonSerializer serializer)
            {
                var contract = serializer.ContractResolver.ResolveContract(objectType);
                if (contract is JsonArrayContract arrayContract)
                    return arrayContract;

                throw new JsonSerializationException("Object is not an array contract");
            }
        }
    }
}
