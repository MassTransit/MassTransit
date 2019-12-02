namespace MassTransit.Serialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Remoting.Messaging;


    public class StaticHeaders :
        Headers
    {
        readonly Header[] _headers;

        public StaticHeaders(Header[] headers)
        {
            _headers = headers;
        }

        IEnumerable<KeyValuePair<string, object>> Headers.GetAll()
        {
            return _headers.Select(x => new KeyValuePair<string, object>(x.Name, x.Value));
        }

        T Headers.Get<T>(string key, T defaultValue)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            object obj;
            if (!TryGetHeader(key, out obj))
                return defaultValue;

            var result = obj as T;

            return result ?? defaultValue;
        }

        public T? Get<T>(string key, T? defaultValue)
            where T : struct
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (!TryGetHeader(key, out var obj))
                return defaultValue;

            if (obj == null)
                return defaultValue;

            if (obj is T value)
                return value;

            return defaultValue;
        }

        public bool TryGetHeader(string key, out object value)
        {
            for (int i = 0; i < _headers.Length; i++)
            {
                if (_headers[i].Name.Equals(key))
                {
                    value = _headers[i].Value;
                    return true;
                }
            }

            value = null;
            return false;
        }

        public IEnumerator<HeaderValue> GetEnumerator()
        {
            return _headers.Select(x => new HeaderValue(x.Name, x.Value)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
