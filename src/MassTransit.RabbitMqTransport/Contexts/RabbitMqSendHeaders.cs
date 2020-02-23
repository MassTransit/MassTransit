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

            if (_basicProperties.Headers == null)
                _basicProperties.Headers = new Dictionary<string, object>();

            if (value == null)
                _basicProperties.Headers.Remove(key);
            else
                _basicProperties.Headers[key] = value;
        }

        public void Set(string key, object value, bool overwrite)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (_basicProperties.Headers == null)
                _basicProperties.Headers = new Dictionary<string, object>();

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

            bool found = _basicProperties.Headers.TryGetValue(key, out value);
            if (found)
            {
                if (value is byte[])
                    value = Encoding.UTF8.GetString((byte[])value);
            }

            return found;
        }

        public IEnumerable<KeyValuePair<string, object>> GetAll()
        {
            if (_basicProperties.IsHeadersPresent())
                return _basicProperties.Headers;

            return Enumerable.Empty<KeyValuePair<string, object>>();
        }

        T MassTransit.Headers.Get<T>(string key, T defaultValue)
        {
            object value;
            if (TryGetHeader(key, out value))
                return ObjectTypeDeserializer.Deserialize(value, defaultValue);

            return defaultValue;
        }

        public T? Get<T>(string key, T? defaultValue)
            where T : struct
        {
            object value;
            if (TryGetHeader(key, out value))
                return ObjectTypeDeserializer.Deserialize(value, defaultValue);

            return defaultValue;
        }

        public IEnumerator<HeaderValue> GetEnumerator()
        {
            if (_basicProperties.IsHeadersPresent())
                return _basicProperties.Headers.Select(x => new HeaderValue(x)).GetEnumerator();

            return Enumerable.Empty<HeaderValue>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
