namespace MassTransit.Serialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Util;


    /// <summary>
    /// The headers stored in the message envelope
    /// </summary>
    public class JsonEnvelopeHeaders :
        Headers
    {
        readonly IDictionary<string, object> _headers;

        public JsonEnvelopeHeaders(IDictionary<string, object> headers)
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

        public T? Get<T>(string key, T? defaultValue = null)
            where T : struct
        {
            return ObjectTypeDeserializer.Deserialize(_headers, key, defaultValue);
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
