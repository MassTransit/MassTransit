namespace MassTransit.Serialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;


    /// <summary>
    /// The transport headers can be retrieved by name, but not enumerated.
    /// </summary>
    public class RawJsonHeaders :
        Headers
    {
        readonly Headers _transportHeaders;

        public RawJsonHeaders(Headers transportHeaders)
        {
            _transportHeaders = transportHeaders;
        }

        public IEnumerable<KeyValuePair<string, object>> GetAll()
        {
            return Enumerable.Empty<KeyValuePair<string, object>>();
        }

        public bool TryGetHeader(string key, out object value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return _transportHeaders.TryGetHeader(key, out value);
        }

        public T Get<T>(string key, T defaultValue)
            where T : class
        {
            return _transportHeaders.Get(key, defaultValue);
        }

        public T? Get<T>(string key, T? defaultValue = null)
            where T : struct
        {
            return _transportHeaders.Get(key, defaultValue);
        }

        public IEnumerator<HeaderValue> GetEnumerator()
        {
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
