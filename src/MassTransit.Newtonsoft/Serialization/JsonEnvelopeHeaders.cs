namespace MassTransit.Serialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;


    /// <summary>
    /// The headers stored in the message envelope
    /// </summary>
    public class JsonEnvelopeHeaders :
        Headers
    {
        readonly IDictionary<string, object> _headers;
        readonly JsonSerializer _serializer;

        public JsonEnvelopeHeaders(IDictionary<string, object> headers, JsonSerializer serializer)
        {
            _serializer = serializer;
            _headers = headers ?? new Dictionary<string, object>();
        }

        public IEnumerable<KeyValuePair<string, object>> GetAll()
        {
            return _headers;
        }

        public bool TryGetHeader(string key, out object value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return _headers.TryGetValue(key, out value);
        }

        public T Get<T>(string key, T defaultValue)
            where T : class
        {
            if (!_headers.TryGetValue(key, out var value) && !_headers.TryGetValueCamelCase(key, out value))
                return defaultValue;

            if (value is T val)
                return val;

            return (T)Deserialize(value, typeof(T));
        }

        public T? Get<T>(string key, T? defaultValue = null)
            where T : struct
        {
            if (!_headers.TryGetValue(key, out var value) && !_headers.TryGetValueCamelCase(key, out value))
                return defaultValue;

            if (value is T val)
                return val;

            return (T)Deserialize(value, typeof(T));
        }

        public IEnumerator<HeaderValue> GetEnumerator()
        {
            return _headers.Select(x => new HeaderValue(x.Key, x.Value)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        object Deserialize(object value, Type objectType, bool allowNull = false)
        {
            var token = value as JToken ?? new JValue(value);
            if (token.Type == JTokenType.Null && allowNull)
                return null;

            if (token.Type == JTokenType.String && objectType.IsInterface && MessageTypeCache.IsValidMessageType(objectType))
                return JsonConvert.DeserializeObject((string)value, objectType, NewtonsoftJsonMessageSerializer.DeserializerSettings);

            using var jsonReader = new JTokenReader(token);

            return _serializer.Deserialize(jsonReader, objectType);
        }
    }
}
