namespace MassTransit.RabbitMqTransport.Contexts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using RabbitMQ.Client;
    using Util;


    public class RabbitMqSendHeaders :
        SendHeaders
    {
        readonly IBasicProperties _basicProperties;

        public RabbitMqSendHeaders(IBasicProperties basicProperties)
        {
            _basicProperties = basicProperties;
        }

        public void Set(string key, string value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            _basicProperties.Headers ??= new Dictionary<string, object>();

            if (value == null)
                _basicProperties.Headers.Remove(key);
            else
                _basicProperties.Headers[key] = value;
        }

        public void Set(string key, object value, bool overwrite)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            _basicProperties.Headers ??= new Dictionary<string, object>();

            if (value == null)
                _basicProperties.Headers.Remove(key);
            else if (overwrite)
                _basicProperties.Headers[key] = value;
            else if (!_basicProperties.Headers.ContainsKey(key))
                _basicProperties.Headers.Add(key, value);
        }

        public bool TryGetHeader(string key, out object value)
        {
            if (_basicProperties.Headers == null)
            {
                value = null;
                return false;
            }

            var found = _basicProperties.Headers.TryGetValue(key, out value);
            if (found)
            {
                if (value is byte[] bytes)
                    value = Encoding.UTF8.GetString(bytes);
            }

            return found;
        }

        public IEnumerable<KeyValuePair<string, object>> GetAll()
        {
            return _basicProperties.IsHeadersPresent() && _basicProperties.Headers != null
                ? _basicProperties.Headers
                : Enumerable.Empty<KeyValuePair<string, object>>();
        }

        T MassTransit.Headers.Get<T>(string key, T defaultValue)
        {
            return TryGetHeader(key, out var value)
                ? ObjectTypeDeserializer.Deserialize(value, defaultValue)
                : defaultValue;
        }

        public T? Get<T>(string key, T? defaultValue)
            where T : struct
        {
            return TryGetHeader(key, out var value)
                ? ObjectTypeDeserializer.Deserialize(value, defaultValue)
                : defaultValue;
        }

        public IEnumerator<HeaderValue> GetEnumerator()
        {
            return _basicProperties.IsHeadersPresent() && _basicProperties.Headers != null
                ? _basicProperties.Headers.Select(x => new HeaderValue(x)).GetEnumerator()
                : Enumerable.Empty<HeaderValue>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
