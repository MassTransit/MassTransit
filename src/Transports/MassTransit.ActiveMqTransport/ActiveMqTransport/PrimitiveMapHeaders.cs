namespace MassTransit.ActiveMqTransport
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Apache.NMS;


    public class PrimitiveMapHeaders :
        SendHeaders
    {
        readonly IPrimitiveMap _properties;

        public PrimitiveMapHeaders(IPrimitiveMap properties)
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

            if (overwrite)
            {
                if (value == null)
                    _properties.Remove(key);
                else
                    _properties[key] = value;
            }
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

        public T Get<T>(string key, T defaultValue)
            where T : class
        {
            throw new NotImplementedByDesignException("PrimitiveMapHeaders does not support object-based header retrieval");
        }

        public T? Get<T>(string key, T? defaultValue)
            where T : struct
        {
            throw new NotImplementedByDesignException("PrimitiveMapHeaders does not support object-based header retrieval");
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
