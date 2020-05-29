namespace MassTransit.Transports
{
    public class StringHeaderValueConverter :
        IHeaderValueConverter
    {
        public bool TryConvert(HeaderValue headerValue, out HeaderValue result)
        {
            if (headerValue.IsStringValue(out HeaderValue<string> stringValue))
            {
                result = stringValue;
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert<T>(HeaderValue<T> headerValue, out HeaderValue result)
        {
            if (headerValue.IsStringValue(out HeaderValue<string> stringValue))
            {
                result = stringValue;
                return true;
            }

            result = default;
            return false;
        }
    }
}
