namespace MassTransit.Serialization
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;


    public class EmptyHeaders :
        Headers
    {
        public static readonly EmptyHeaders Instance = new EmptyHeaders();

        EmptyHeaders()
        {
        }

        public IEnumerable<KeyValuePair<string, object>> GetAll()
        {
            return Enumerable.Empty<KeyValuePair<string, object>>();
        }

        public bool TryGetHeader(string key, out object? value)
        {
            value = default;
            return false;
        }

        public T? Get<T>(string key, T? defaultValue)
            where T : class
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
