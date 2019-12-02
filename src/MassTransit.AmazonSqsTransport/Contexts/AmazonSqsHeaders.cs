namespace MassTransit.AmazonSqsTransport.Contexts
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Amazon.SQS.Model;
    using Util;


    public class AmazonSqsHeaders :
        Headers
    {
        readonly IDictionary<string, MessageAttributeValue> _attributes;

        public AmazonSqsHeaders(IDictionary<string, MessageAttributeValue> attributes)
        {
            _attributes = attributes;
        }

        public bool TryGetHeader(string key, out object value)
        {
            if (_attributes.TryGetValue(key, out var val))
            {
                value = val.StringValue;
                return value != null;
            }

            value = null;
            return false;
        }

        public IEnumerable<KeyValuePair<string, object>> GetAll()
        {
            return _attributes.Where(x => x.Value.StringValue != null).Select(x => new KeyValuePair<string, object>(x.Key, x.Value.StringValue));
        }

        T Headers.Get<T>(string key, T defaultValue)
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
            return _attributes.Where(x => x.Value.StringValue != null).Select(x => new HeaderValue(x.Key, x.Value.StringValue)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
