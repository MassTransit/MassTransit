namespace MassTransit.Transports
{
    public class SimpleHeaderValueConverter :
        IHeaderValueConverter
    {
        public bool TryConvert(HeaderValue headerValue, out HeaderValue result)
        {
            if (headerValue.IsSimpleValue(out var simpleValue))
            {
                result = simpleValue;
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert<T>(HeaderValue<T> headerValue, out HeaderValue result)
        {
            if (headerValue.IsSimpleValue(out var simpleValue))
            {
                result = simpleValue;
                return true;
            }

            result = default;
            return false;
        }
    }
}
