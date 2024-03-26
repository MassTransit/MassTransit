namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Serialization;
    using Transports;


    public static class SerializerContextExtensions
    {
        public static T? GetValue<T>(this IObjectDeserializer context, IReadOnlyDictionary<string, object> dictionary, string key, T? defaultValue = null)
            where T : class
        {
            if (!dictionary.TryGetValue(key, out var value) && !dictionary.TryGetValueCamelCase(key, out value))
                return defaultValue;

            return context.DeserializeObject(value, defaultValue);
        }

        public static T? GetValue<T>(this IObjectDeserializer context, IReadOnlyDictionary<string, object> dictionary, string key, T? defaultValue = null)
            where T : struct
        {
            if (!dictionary.TryGetValue(key, out var value) && !dictionary.TryGetValueCamelCase(key, out value))
                return defaultValue;

            return context.DeserializeObject(value, defaultValue);
        }

        public static T? GetValue<T>(this IObjectDeserializer context, IDictionary<string, object> dictionary, string key, T? defaultValue = null)
            where T : class
        {
            if (!dictionary.TryGetValue(key, out var value) && !dictionary.TryGetValueCamelCase(key, out value))
                return defaultValue;

            return context.DeserializeObject(value, defaultValue);
        }

        public static T? GetValue<T>(this IObjectDeserializer context, IDictionary<string, object> dictionary, string key, T? defaultValue = null)
            where T : struct
        {
            if (!dictionary.TryGetValue(key, out var value) && !dictionary.TryGetValueCamelCase(key, out value))
                return defaultValue;

            return context.DeserializeObject(value, defaultValue);
        }

        public static T? GetValue<T>(this IObjectDeserializer context, IHeaderProvider dictionary, string key, T? defaultValue = null)
            where T : class
        {
            return dictionary.TryGetHeader(key, out var value) ? context.DeserializeObject(value, defaultValue) : defaultValue;
        }

        public static T? GetValue<T>(this IObjectDeserializer context, IHeaderProvider dictionary, string key, T? defaultValue = null)
            where T : struct
        {
            return dictionary.TryGetHeader(key, out var value) ? context.DeserializeObject(value, defaultValue) : defaultValue;
        }

        public static bool TryGetValue<T>(this IObjectDeserializer context, IDictionary<string, object> dictionary, string key,
            [NotNullWhen(true)] out T? value)
            where T : class
        {
            if (!dictionary.TryGetValue(key, out var obj) && !dictionary.TryGetValueCamelCase(key, out obj))
            {
                value = null;
                return false;
            }

            value = context.DeserializeObject<T>(obj);
            return value != null;
        }

        public static string? SerializeDictionary(this IObjectDeserializer deserializer, IEnumerable<KeyValuePair<string, object>> values)
        {
            var dictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            foreach (KeyValuePair<string, object> pair in values)
            {
                if (pair.Value != null)
                    dictionary[pair.Key] = pair.Value;
            }

            return dictionary.Count == 0
                ? null
                : deserializer.SerializeObject(dictionary).GetString();
        }

        public static Dictionary<string, TValue>? DeserializeDictionary<TValue>(this IObjectDeserializer deserializer, string? text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                List<KeyValuePair<string, TValue>>? headers = deserializer.DeserializeObject<IEnumerable<KeyValuePair<string, TValue>>>(text)?.ToList();
                if (headers != null && headers.Count > 0)
                {
                    var dictionary = new Dictionary<string, TValue>(StringComparer.OrdinalIgnoreCase);
                    foreach (KeyValuePair<string, TValue> x in headers)
                        dictionary.Add(x.Key, x.Value);

                    return dictionary;
                }
            }

            return null;
        }

        public static bool TryGetValue<T>(this IObjectDeserializer context, IDictionary<string, object> dictionary, string key,
            [NotNullWhen(true)] out T? value)
            where T : struct
        {
            if (!dictionary.TryGetValue(key, out var obj) && !dictionary.TryGetValueCamelCase(key, out obj))
            {
                value = null;
                return false;
            }

            value = context.DeserializeObject<T>(obj);
            return value != null;
        }

        public static bool TryGetHeader<T>(this ConsumeContext context, string key, [NotNullWhen(true)] out T? value)
            where T : class
        {
            if (!context.Headers.TryGetHeader(key, out var headerValue))
            {
                value = null;
                return false;
            }

            value = context.SerializerContext.DeserializeObject<T>(headerValue);
            return value != null;
        }

        public static bool TryGetHeader<T>(this ConsumeContext context, string key, [NotNullWhen(true)] out T? value)
            where T : struct
        {
            if (!context.Headers.TryGetHeader(key, out var headerValue))
            {
                value = null;
                return false;
            }

            value = context.SerializerContext.DeserializeObject<T>(headerValue);
            return value != null;
        }

        public static bool TryGetHeader<T>(this SendContext context, string key, [NotNullWhen(true)] out T? value)
            where T : class
        {
            if (context.Headers.TryGetHeader(key, out var headerValue))
            {
                value = headerValue as T;
                return value != null;
            }

            value = null;
            return false;
        }

        public static bool TryGetHeader<T>(this SendContext context, string key, [NotNullWhen(true)] out T? value)
            where T : struct
        {
            if (context.Headers.TryGetHeader(key, out var headerValue))
            {
                value = headerValue as T?;
                return value != null;
            }

            value = null;
            return false;
        }

        public static string? GetHeader(this ConsumeContext context, string key, string? defaultValue = null)
        {
            if (!context.Headers.TryGetHeader(key, out var headerValue))
                return defaultValue;

            if (headerValue is string text)
                return text;

            return context.SerializerContext.DeserializeObject<string>(headerValue) ?? defaultValue;
        }

        public static T? GetHeader<T>(this ConsumeContext context, string key, T? defaultValue = null)
            where T : class
        {
            if (!context.Headers.TryGetHeader(key, out var headerValue))
                return defaultValue;

            return context.SerializerContext.DeserializeObject<T>(headerValue) ?? defaultValue;
        }

        public static T? GetHeader<T>(this ConsumeContext context, string key, T? defaultValue = null)
            where T : struct
        {
            if (!context.Headers.TryGetHeader(key, out var headerValue))
                return defaultValue;

            return context.SerializerContext.DeserializeObject<T>(headerValue) ?? defaultValue;
        }

        public static Dictionary<string, object> ToDictionary<T>(this ConsumeContext context, T value)
            where T : class
        {
            return context.SerializerContext.ToDictionary(value);
        }
    }
}
