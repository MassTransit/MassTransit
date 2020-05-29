namespace MassTransit.ActiveMqTransport.Contexts
{
    using System;
    using System.Collections.Generic;
    using Apache.NMS;
    using Initializers.TypeConverters;


    public static class TransportHeaderExtensions
    {
        static readonly DateTimeOffsetTypeConverter _dateTimeOffsetConverter = new DateTimeOffsetTypeConverter();
        static readonly DateTimeTypeConverter _dateTimeConverter = new DateTimeTypeConverter();

        public static void SetHeaders(this IPrimitiveMap dictionary, SendHeaders headers)
        {
            foreach (KeyValuePair<string, object> header in headers.GetAll())
            {
                if (header.Value == null)
                {
                    if (dictionary.Contains(header.Key))
                        dictionary.Remove(header.Key);

                    continue;
                }

                if (dictionary.Contains(header.Key))
                    continue;

                switch (header.Value)
                {
                    case DateTimeOffset dateTimeOffset:
                        if (_dateTimeOffsetConverter.TryConvert(dateTimeOffset, out long result))
                            dictionary[header.Key] = result;
                        else if (_dateTimeOffsetConverter.TryConvert(dateTimeOffset, out string text))
                            dictionary[header.Key] = text;

                        break;

                    case DateTime dateTime:
                        if (_dateTimeConverter.TryConvert(dateTime, out result))
                            dictionary[header.Key] = result;
                        else if (_dateTimeConverter.TryConvert(dateTime, out string text))
                            dictionary[header.Key] = text;

                        break;

                    case string s:
                        dictionary[header.Key] = s;
                        break;

                    case IFormattable formatValue:
                        if (header.Value.GetType().IsValueType)
                            dictionary[header.Key] = header.Value;
                        else
                            dictionary[header.Key] = formatValue.ToString();
                        break;
                }
            }
        }
    }
}
