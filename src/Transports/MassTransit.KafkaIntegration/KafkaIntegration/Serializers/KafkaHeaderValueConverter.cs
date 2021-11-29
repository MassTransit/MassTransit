namespace MassTransit.KafkaIntegration.Serializers
{
    using System;
    using System.Text;
    using System.Text.Json;
    using Confluent.Kafka;
    using Serialization;
    using Transports;


    public class KafkaHeaderValueConverter :
        IHeaderValueConverter<Header>
    {
        readonly AllowTransportHeader _allowTransportHeader;

        public KafkaHeaderValueConverter(AllowTransportHeader allowTransportHeader = default)
        {
            _allowTransportHeader = allowTransportHeader ?? AlwaysCopy;
        }

        public bool TryConvert(HeaderValue headerValue, out HeaderValue<Header> result)
        {
            if (headerValue.IsStringValue(out HeaderValue<string> stringValue) && _allowTransportHeader(stringValue))
            {
                result = CreateHeaderValue(stringValue);
                return true;
            }

            if (headerValue.Value != null)
            {
                result = new HeaderValue<Header>(headerValue.Key, new Header(headerValue.Key, Serialize(headerValue.Value)));
                return true;
            }


            result = default;
            return false;
        }

        public bool TryConvert<T>(HeaderValue<T> headerValue, out HeaderValue<Header> result)
        {
            if (headerValue.IsStringValue(out HeaderValue<string> stringValue) && _allowTransportHeader(stringValue))
            {
                result = CreateHeaderValue(stringValue);
                return true;
            }

            if (headerValue.Value != null)
            {
                result = new HeaderValue<Header>(headerValue.Key, new Header(headerValue.Key, Serialize(headerValue.Value)));
                return true;
            }

            result = default;
            return false;
        }

        static HeaderValue<Header> CreateHeaderValue(HeaderValue<string> stringValue)
        {
            return new HeaderValue<Header>(stringValue.Key, new Header(stringValue.Key, Encoding.UTF8.GetBytes(stringValue.Value)));
        }

        static bool AlwaysCopy(HeaderValue<string> headerValue)
        {
            return true;
        }

        byte[] Serialize(object value, Type valueType = default)
        {
            return JsonSerializer.SerializeToUtf8Bytes(value, valueType ?? value.GetType(), SystemTextJsonMessageSerializer.Options);
        }

        byte[] Serialize<T>(T value)
        {
            return Serialize(value, typeof(T));
        }
    }
}
