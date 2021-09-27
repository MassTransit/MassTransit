namespace MassTransit.Util
{
    using System;
    using System.Collections.Generic;
    using Context;
    using Courier;
    using Metadata;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Serialization;


    /// <summary>
    /// Support deserialization of 'objects' from messages into actual types. Objects should have been
    /// serialized with JSON.NET (or some similar serializer).
    /// </summary>
    public class ObjectTypeDeserializer :
        IObjectTypeDeserializer
    {
        ObjectTypeDeserializer()
        {
        }

        public static IObjectTypeDeserializer Instance => Cached.Deserializer.Value;

        T IObjectTypeDeserializer.Deserialize<T>(IDictionary<string, object> dictionary, string key, T defaultValue)
        {
            if (!dictionary.TryGetValue(key, out var value) && !dictionary.TryGetValueCamelCase(key, out value))
                return defaultValue;

            return Deserialize<T>(value);
        }

        T IObjectTypeDeserializer.Deserialize<T>(IDictionary<string, object> dictionary, string key)
        {
            if (!dictionary.TryGetValue(key, out var value) && !dictionary.TryGetValueCamelCase(key, out value))
                throw new KeyNotFoundException($"The key was not present: {key}");

            return Deserialize<T>(value);
        }

        T IObjectTypeDeserializer.Deserialize<T>(IDictionary<string, object> dictionary)
        {
            var jsonObject = JObject.FromObject(dictionary ?? new Dictionary<string, object>());

            using var jsonReader = new JTokenReader(jsonObject);

            return SerializerCache.Deserializer.Deserialize<T>(jsonReader);
        }

        T IObjectTypeDeserializer.Deserialize<T>(IHeaderProvider dictionary, string key, T defaultValue)
        {
            return dictionary.TryGetHeader(key, out var value)
                ? Deserialize<T>(value)
                : defaultValue;
        }

        public T Deserialize<T>(object value)
        {
            if (value is T val)
                return val;

            return (T)Deserialize(value, typeof(T), false);
        }

        T IObjectTypeDeserializer.Deserialize<T>(object value, T defaultValue)
        {
            if (value is T val)
                return val;

            var result = Deserialize(value, typeof(T), true);
            if (result == null)
                return defaultValue;

            return (T)result;
        }

        public object Deserialize(object value, Type objectType, bool allowNull = false)
        {
            var token = value as JToken ?? new JValue(value);
            if (token.Type == JTokenType.Null && allowNull)
                return null;

            if (token.Type == JTokenType.String && objectType.IsInterface && TypeMetadataCache.IsValidMessageType(objectType))
                return JsonConvert.DeserializeObject((string)value, objectType, JsonMessageSerializer.DeserializerSettings);

            using var jsonReader = new JTokenReader(token);

            return SerializerCache.Deserializer.Deserialize(jsonReader, objectType);
        }

        public static T Deserialize<T>(IDictionary<string, object> dictionary, string key)
        {
            return Cached.Deserializer.Value.Deserialize<T>(dictionary, key);
        }

        public static T Deserialize<T>(IDictionary<string, object> dictionary)
        {
            return Cached.Deserializer.Value.Deserialize<T>(dictionary);
        }

        public static T Deserialize<T>(IDictionary<string, object> dictionary, string key, T defaultValue)
        {
            return Cached.Deserializer.Value.Deserialize(dictionary, key, defaultValue);
        }

        public static T Deserialize<T>(IHeaderProvider dictionary, string key, T defaultValue)
        {
            return Cached.Deserializer.Value.Deserialize(dictionary, key, defaultValue);
        }

        public static T Deserialize<T>(object value, T defaultValue)
        {
            return Cached.Deserializer.Value.Deserialize(value, defaultValue);
        }


        static class Cached
        {
            internal static readonly Lazy<IObjectTypeDeserializer> Deserializer = new Lazy<IObjectTypeDeserializer>(() => new ObjectTypeDeserializer());
        }
    }
}
