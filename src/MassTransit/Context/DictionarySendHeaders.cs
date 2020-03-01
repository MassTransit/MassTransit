namespace MassTransit.Context
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Util;


    public class DictionarySendHeaders :
        SendHeaders
    {
        readonly IDictionary<string, object> _headers;

        public DictionarySendHeaders()
        {
            _headers = new Dictionary<string, object>();
        }

        public DictionarySendHeaders(IDictionary<string, object> dictionary)
        {
            _headers = dictionary;
        }

        void SendHeaders.Set(string key, string value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null)
                _headers.Remove(key);
            else
                _headers[key] = value;
        }

        void SendHeaders.Set(string key, object value, bool overwrite)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null)
                _headers.Remove(key);
            else if (overwrite)
                _headers[key] = value;
            else if (!_headers.ContainsKey(key))
                _headers.Add(key, value);
        }

        bool Headers.TryGetHeader(string key, out object value)
        {
            return _headers.TryGetValue(key, out value);
        }

        IEnumerable<KeyValuePair<string, object>> Headers.GetAll()
        {
            return _headers;
        }

        T Headers.Get<T>(string key, T defaultValue)
        {
            return ObjectTypeDeserializer.Deserialize(_headers, key, defaultValue);
        }

        public T? Get<T>(string key, T? defaultValue)
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
