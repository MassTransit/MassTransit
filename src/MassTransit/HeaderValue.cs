namespace MassTransit
{
    using System;
    using System.Collections.Generic;


    public readonly struct HeaderValue<T>
    {
        public readonly string Key;
        public readonly T Value;

        public HeaderValue(string key, T value)
        {
            Key = key;
            Value = value;
        }

        public bool IsStringValue(out HeaderValue<string> result)
        {
            if (this is HeaderValue<string> resultValue)
            {
                result = resultValue;
                return true;
            }

            switch (Value)
            {
                case null:
                    result = default;
                    return false;
                case string stringValue:
                    result = new HeaderValue<string>(Key, stringValue);
                    return true;
                case bool boolValue when boolValue:
                    result = new HeaderValue<string>(Key, bool.TrueString);
                    return true;
                case IFormattable formatValue when formatValue.GetType().IsValueType:
                    result = new HeaderValue<string>(Key, formatValue.ToString());
                    return true;
                default:
                    result = default;
                    return false;
            }
        }
    }


    public readonly struct HeaderValue
    {
        public readonly string Key;
        public readonly object Value;

        public HeaderValue(string key, object value)
        {
            Key = key;
            Value = value;
        }

        public HeaderValue(KeyValuePair<string, object> pair)
        {
            Key = pair.Key;
            Value = pair.Value;
        }

        public bool IsStringValue(out HeaderValue<string> result)
        {
            switch (Value)
            {
                case null:
                    result = default;
                    return false;
                case string stringValue:
                    result = new HeaderValue<string>(Key, stringValue);
                    return true;
                case bool boolValue when boolValue:
                    result = new HeaderValue<string>(Key, bool.TrueString);
                    return true;
                case IFormattable formatValue when formatValue.GetType().IsValueType:
                    result = new HeaderValue<string>(Key, formatValue.ToString());
                    return true;
                default:
                    result = default;
                    return false;
            }
        }

        public static implicit operator HeaderValue(HeaderValue<string> headerValue)
        {
            return new HeaderValue(headerValue.Key, headerValue.Value);
        }
    }
}
