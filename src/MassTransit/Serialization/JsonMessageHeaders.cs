namespace MassTransit.Serialization
{
    using System;
    using System.Collections.Generic;
    using Util;


    public class JsonMessageHeaders :
        Headers
    {
        readonly IDictionary<string, object> _headers;

        public JsonMessageHeaders(IDictionary<string, object> headers)
        {
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

        T Headers.Get<T>(string key, T defaultValue)
        {
            return ObjectTypeDeserializer.Deserialize(_headers, key, defaultValue);
        }

        public T? Get<T>(string key, T? defaultValue = null) where T : struct
        {
            return ObjectTypeDeserializer.Deserialize(_headers, key, defaultValue);
        }
    }
}
