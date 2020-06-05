namespace MassTransit.KafkaIntegration.Serializers
{
    using System;
    using System.IO;
    using System.Text;
    using Confluent.Kafka;
    using Courier;
    using Newtonsoft.Json;
    using Transports;


    public class KafkaHeaderValueConverter :
        IHeaderValueConverter<Header>
    {
        readonly AllowTransportHeader _allowTransportHeader;
        readonly Encoding _encoding;

        public KafkaHeaderValueConverter(Encoding encoding, AllowTransportHeader allowTransportHeader = default)
        {
            _encoding = encoding;
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
            using var stream = new MemoryStream();
            using var writer = new StreamWriter(stream, _encoding, 1024, true);
            using var jsonWriter = new JsonTextWriter(writer) {Formatting = Formatting.None};

            SerializerCache.Serializer.Serialize(jsonWriter, value, valueType ?? value.GetType());

            jsonWriter.Flush();
            writer.Flush();

            return stream.ToArray();
        }

        byte[] Serialize<T>(T value)
        {
            return Serialize(value, typeof(T));
        }
    }
}
