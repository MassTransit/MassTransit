namespace MassTransit.Transports
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Serialization;


    /// <summary>
    /// The context headers are sourced from the IContextHeaderProvider, with the use of a Json deserializer
    /// to convert data types to objects as required. If the original headers are Json objects, those headers
    /// are deserialized as well
    /// </summary>
    public class JsonTransportHeaders :
        Headers
    {
        readonly IHeaderProvider _provider;

        public JsonTransportHeaders(IHeaderProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public IEnumerable<KeyValuePair<string, object>> GetAll()
        {
            return _provider.GetAll();
        }

        public bool TryGetHeader(string key, out object value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return _provider.TryGetHeader(key, out value);
        }

        public T Get<T>(string key, T defaultValue)
            where T : class
        {
            return SystemTextJsonMessageSerializer.Instance.GetValue(_provider, key, defaultValue);
        }

        public T? Get<T>(string key, T? defaultValue)
            where T : struct
        {
            return SystemTextJsonMessageSerializer.Instance.GetValue(_provider, key, defaultValue);
        }

        public IEnumerator<HeaderValue> GetEnumerator()
        {
            return _provider.GetAll().Select(x => new HeaderValue(x.Key, x.Value)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
