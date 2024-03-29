namespace MassTransit.Initializers.TypeConverters
{
    using System;
    using Internals;


    public class DateTimeOffsetTypeConverter :
        ITypeConverter<string, DateTimeOffset>,
        ITypeConverter<int, DateTimeOffset>,
        ITypeConverter<long, DateTimeOffset>,
        ITypeConverter<DateTimeOffset, string>,
        ITypeConverter<DateTimeOffset, object>
    {
        public bool TryConvert(object input, out DateTimeOffset result)
        {
            switch (input)
            {
                case DateTime dateTime:
                    result = dateTime;
                    return true;

                case DateTimeOffset dateTimeOffset:
                    result = dateTimeOffset;
                    return true;

                case string text when !string.IsNullOrWhiteSpace(text):
                    return TryConvert(text, out result);

                default:
                    result = default;
                    return false;
            }
        }

        public bool TryConvert(string input, out DateTimeOffset result)
        {
            return DateTimeOffset.TryParse(input, out result);
        }

        public bool TryConvert(DateTimeOffset input, out int result)
        {
            if (input >= DateTimeConstants.Epoch)
            {
                var timeSpan = input.UtcDateTime - DateTimeConstants.Epoch;
                if (timeSpan.TotalMilliseconds <= int.MaxValue)
                {
                    result = (int)timeSpan.TotalMilliseconds;
                    return true;
                }
            }

            result = default;
            return false;
        }

        public bool TryConvert(DateTimeOffset input, out long result)
        {
            if (input >= DateTimeConstants.Epoch)
            {
                var timeSpan = input.UtcDateTime - DateTimeConstants.Epoch;
                if (timeSpan.TotalMilliseconds <= long.MaxValue)
                {
                    result = (long)timeSpan.TotalMilliseconds;
                    return true;
                }
            }

            result = default;
            return false;
        }

        public bool TryConvert(DateTimeOffset input, out string result)
        {
            result = input.ToString("O");
            return true;
        }
    }
}
