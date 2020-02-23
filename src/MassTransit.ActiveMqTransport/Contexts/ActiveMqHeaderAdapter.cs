namespace MassTransit.ActiveMqTransport.Contexts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Apache.NMS;
    using Util;


    public class ActiveMqHeaderAdapter :
        SendHeaders
    {
        readonly IPrimitiveMap _properties;

        public ActiveMqHeaderAdapter(IPrimitiveMap properties)
        {
            _properties = properties;
        }

        public void Set(string key, string value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null)
                _properties.Remove(key);
            else
                _properties[key] = value;
        }

        public void Set(string key, object value, bool overwrite)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null)
                _properties.Remove(key);
            else if (overwrite)
                _properties[key] = value;
            else if (!_properties.Contains(key))
                _properties[key] = value;
        }

        public bool TryGetHeader(string key, out object value)
        {
            var found = _properties.Contains(key);
            if (found)
            {
                value = _properties[key];
                return true;
            }

            value = null;
            return false;
        }

        public IEnumerable<KeyValuePair<string, object>> GetAll()
        {
            foreach (string key in _properties.Keys)
            {
                var value = _properties[key];

                yield return new KeyValuePair<string, object>(key, value);
            }
        }

        T Headers.Get<T>(string key, T defaultValue)
        {
            if (TryGetHeader(key, out var value))
                return ObjectTypeDeserializer.Deserialize(value, defaultValue);

            return defaultValue;
        }

        public T? Get<T>(string key, T? defaultValue)
            where T : struct
        {
            if (TryGetHeader(key, out var value))
                return ObjectTypeDeserializer.Deserialize(value, defaultValue);

            return defaultValue;
        }

        public IEnumerator<HeaderValue> GetEnumerator()
        {
            foreach (string key in _properties.Keys)
            {
                var value = _properties[key];

                if (value != null)
                    yield return new HeaderValue(key, value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
