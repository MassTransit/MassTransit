#nullable enable
namespace MassTransit.Serialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;


    public class DictionarySendHeaders :
        SendHeaders
    {
        readonly IDictionary<string, object> _headers;

        public DictionarySendHeaders()
        {
            _headers = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public void Set(string key, string? value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null)
                _headers.Remove(key);
            else
                _headers[key] = value;
        }

        public void Set(string key, object? value, bool overwrite = true)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (overwrite)
            {
                if (value == null)
                    _headers.Remove(key);
                else
                    _headers[key] = value;
            }
            else if (!_headers.ContainsKey(key) && value != null)
                _headers.Add(key, value);
        }

        public bool TryGetHeader(string key, out object value)
        {
            return _headers.TryGetValue(key, out value);
        }

        public IEnumerable<KeyValuePair<string, object>> GetAll()
        {
            return _headers;
        }

        public T? Get<T>(string key, T? defaultValue)
            where T : class
        {
            return SystemTextJsonMessageSerializer.Instance.GetValue(_headers, key, defaultValue);
        }

        public T? Get<T>(string key, T? defaultValue)
            where T : struct
        {
            return SystemTextJsonMessageSerializer.Instance.GetValue(_headers, key, defaultValue);
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
