namespace MassTransit.Serialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;


    /// <summary>
    /// When a message envelope is deserialized, encapsulate the headers such that objects can be deserialized from the
    /// body using the message deserializer.
    /// </summary>
    public class ReadOnlyDictionaryHeaders :
        Headers
    {
        readonly IObjectDeserializer _deserializer;
        readonly IReadOnlyDictionary<string, object> _headers;

        public ReadOnlyDictionaryHeaders(IObjectDeserializer deserializer, IReadOnlyDictionary<string, object> headers)
        {
            _deserializer = deserializer;

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
            return _deserializer.GetValue(_headers, key, defaultValue);
        }

        public T? Get<T>(string key, T? defaultValue = null)
            where T : struct
        {
            return _deserializer.GetValue(_headers, key, defaultValue);
        }

        public IEnumerator<HeaderValue> GetEnumerator()
        {
            return _headers.Select(x => new HeaderValue(x.Key, x.Value)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
