namespace MassTransit.Serialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;


    public class NoMessageHeaders :
        Headers
    {
        public static readonly NoMessageHeaders Instance = new NoMessageHeaders();

        NoMessageHeaders()
        {
        }

        public IEnumerable<KeyValuePair<string, object>> GetAll()
        {
            return Enumerable.Empty<KeyValuePair<string, object>>();
        }

        public bool TryGetHeader(string key, out object value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            value = default;
            return false;
        }

        T Headers.Get<T>(string key, T defaultValue)
        {
            return defaultValue;
        }

        public T? Get<T>(string key, T? defaultValue = null)
            where T : struct
        {
            return defaultValue;
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
