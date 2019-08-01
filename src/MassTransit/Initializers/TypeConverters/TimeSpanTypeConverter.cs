namespace MassTransit.Initializers.TypeConverters
{
    using System;


    public class TimeSpanTypeConverter :
        ITypeConverter<string, TimeSpan>,
        ITypeConverter<TimeSpan, string>,
        ITypeConverter<TimeSpan, object>,
        ITypeConverter<TimeSpan, sbyte>,
        ITypeConverter<TimeSpan, byte>,
        ITypeConverter<TimeSpan, short>,
        ITypeConverter<TimeSpan, ushort>,
        ITypeConverter<TimeSpan, int>,
        ITypeConverter<TimeSpan, uint>,
        ITypeConverter<TimeSpan, long>,
        ITypeConverter<TimeSpan, ulong>,
        ITypeConverter<TimeSpan, double>

    {
        public bool TryConvert(string input, out TimeSpan result)
        {
            return TimeSpan.TryParse(input, out result);
        }

        public bool TryConvert(short input, out TimeSpan result)
        {
            result = TimeSpan.FromMilliseconds(input);
            return true;
        }

        public bool TryConvert(int input, out TimeSpan result)
        {
            result = TimeSpan.FromMilliseconds(input);
            return true;
        }

        public bool TryConvert(long input, out TimeSpan result)
        {
            result = TimeSpan.FromMilliseconds(input);
            return true;
        }

        public bool TryConvert(double input, out TimeSpan result)
        {
            result = TimeSpan.FromMilliseconds(input);
            return true;
        }

        public bool TryConvert(sbyte input, out TimeSpan result)
        {
            result = TimeSpan.FromMilliseconds(input);
            return true;
        }

        public bool TryConvert(byte input, out TimeSpan result)
        {
            result = TimeSpan.FromMilliseconds(input);
            return true;
        }

        public bool TryConvert(ushort input, out TimeSpan result)
        {
            result = TimeSpan.FromMilliseconds(input);
            return true;
        }

        public bool TryConvert(uint input, out TimeSpan result)
        {
            result = TimeSpan.FromMilliseconds(input);
            return true;
        }

        public bool TryConvert(ulong input, out TimeSpan result)
        {
            result = TimeSpan.FromMilliseconds(input);
            return true;
        }

        public bool TryConvert(TimeSpan input, out string result)
        {
            result = input.ToString("c");
            return true;
        }

        public bool TryConvert(object input, out TimeSpan result)
        {
            switch (input)
            {
                case TimeSpan timeSpan:
                    result = timeSpan;
                    return true;

                case string text when !string.IsNullOrWhiteSpace(text):
                    return TryConvert(text, out result);

                default:
                    result = default;
                    return false;
            }
        }
    }
}
