namespace MassTransit.AmazonSqsTransport.Contexts
{
    using System;
    using System.Collections.Generic;
    using Amazon.SimpleNotificationService.Model;
    using Util;


    public class AmazonSnsHeaderAdapter :
        SendHeaders
    {
        readonly IDictionary<string, MessageAttributeValue> _attributes;

        public AmazonSnsHeaderAdapter(IDictionary<string, MessageAttributeValue> attributes)
        {
            _attributes = attributes;
        }

        public void Set(string key, string value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null)
                _attributes.Remove(key);
            else
                _attributes[key].StringValue = value;
        }

        public void Set(string key, object value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null)
                _attributes.Remove(key);
            else
                _attributes[key].StringValue = value.ToString();
        }

        public bool TryGetHeader(string key, out object value)
        {
            var found = _attributes.ContainsKey(key);
            if (found)
            {
                value = _attributes[key].StringValue;
                return true;
            }

            value = null;
            return false;
        }

        public IEnumerable<KeyValuePair<string, object>> GetAll()
        {
            foreach (string key in _attributes.Keys)
            {
                var value = _attributes[key];

                yield return new KeyValuePair<string, object>(key, value);
            }
        }

        T Headers.Get<T>(string key, T defaultValue)
        {
            if (TryGetHeader(key, out var value))
                return ObjectTypeDeserializer.Deserialize(value, defaultValue);

            return defaultValue;
        }

        T? Headers.Get<T>(string key, T? defaultValue)
        {
            if (TryGetHeader(key, out var value))
                return ObjectTypeDeserializer.Deserialize(value, defaultValue);

            return defaultValue;
        }
    }
}