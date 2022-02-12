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
            switch (this)
            {
                case HeaderValue<string> resultValue:
                    result = resultValue;
                    return true;
                default:
                    return HeaderValue.IsValueStringValue(Key, Value, out result);
            }
        }

        public bool IsSimpleValue(out HeaderValue result)
        {
            return HeaderValue.IsValueSimpleValue(Key, Value, out result);
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
            return IsValueStringValue(Key, Value, out result);
        }

        public bool IsSimpleValue(out HeaderValue result)
        {
            return IsValueSimpleValue(Key, Value, out result);
        }

        public static implicit operator HeaderValue(HeaderValue<string> headerValue)
        {
            return new HeaderValue(headerValue.Key, headerValue.Value);
        }

        internal static bool IsValueStringValue(string key, object? value, out HeaderValue<string> result)
        {
            switch (value)
            {
                case null:
                    result = default;
                    return false;
                case string stringValue:
                    result = new HeaderValue<string>(key, stringValue);
                    return true;
                case bool boolValue when boolValue:
                    result = new HeaderValue<string>(key, bool.TrueString);
                    return true;
                case Uri uri:
                    result = new HeaderValue<string>(key, uri.ToString());
                    return true;
                case IFormattable formatValue when formatValue.GetType().IsValueType:
                    result = new HeaderValue<string>(key, formatValue.ToString());
                    return true;
                default:
                    result = default;
                    return false;
            }
        }

        internal static bool IsValueSimpleValue(string key, object? value, out HeaderValue result)
        {
            switch (value)
            {
                case null:
                    result = default;
                    return false;
                case string stringValue:
                    result = new HeaderValue<string>(key, stringValue);
                    return true;
                case bool boolValue when boolValue:
                    result = new HeaderValue(key, true);
                    return true;
                case Uri uri:
                    result = new HeaderValue<string>(key, uri.ToString());
                    return true;
                case IFormattable formatValue when formatValue.GetType().IsValueType:
                    result = new HeaderValue(key, value);
                    return true;
                default:
                    result = default;
                    return false;
            }
        }
    }
}
