namespace MassTransit.AmazonSqsTransport
{
    using Amazon.SimpleNotificationService.Model;
    using Transports;


    public class SnsHeaderValueConverter :
        IHeaderValueConverter<MessageAttributeValue>
    {
        readonly AllowTransportHeader _allowTransportHeader;

        public SnsHeaderValueConverter(AllowTransportHeader allowTransportHeader = default)
        {
            _allowTransportHeader = allowTransportHeader ?? AlwaysCopy;
        }

        public bool TryConvert(HeaderValue headerValue, out HeaderValue<MessageAttributeValue> result)
        {
            if (headerValue.IsStringValue(out HeaderValue<string> stringValue) && _allowTransportHeader(stringValue))
            {
                result = CreateMessageAttributeValue(stringValue);
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert<T>(HeaderValue<T> headerValue, out HeaderValue<MessageAttributeValue> result)
        {
            if (headerValue.IsStringValue(out HeaderValue<string> stringValue) && _allowTransportHeader(stringValue))
            {
                result = CreateMessageAttributeValue(stringValue);
                return true;
            }

            result = default;
            return false;
        }

        static HeaderValue<MessageAttributeValue> CreateMessageAttributeValue(HeaderValue<string> stringValue)
        {
            return new HeaderValue<MessageAttributeValue>(stringValue.Key, new MessageAttributeValue
            {
                StringValue = stringValue.Value,
                DataType = "String"
            });
        }

        static bool AlwaysCopy(HeaderValue<string> headerValue)
        {
            return true;
        }
    }
}
