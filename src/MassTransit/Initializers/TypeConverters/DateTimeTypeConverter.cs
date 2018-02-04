namespace MassTransit.Initializers.TypeConverters
{
    using System;


    public class DateTimeTypeConverter :
        ITypeConverter<string, DateTime>,
        ITypeConverter<DateTime, string>,
        ITypeConverter<DateTime, DateTimeOffset>,
        ITypeConverter<DateTime, int>,
        ITypeConverter<DateTime, long>
    {
        readonly DateTime _epoch = new DateTime(1970, 1, 1);

        public bool TryConvert(DateTime input, out string result)
        {
            result = input.ToString("O");
            return true;
        }

        public bool TryConvert(string input, out DateTime result)
        {
            return DateTime.TryParse(input, out result);
        }

        public bool TryConvert(DateTimeOffset input, out DateTime result)
        {
            result = input.UtcDateTime;
            return true;
        }

        public bool TryConvert(int input, out DateTime result)
        {
            result = _epoch + TimeSpan.FromMilliseconds(input);
            return true;
        }

        public bool TryConvert(long input, out DateTime result)
        {
            result = _epoch + TimeSpan.FromMilliseconds(input);
            return true;
        }
    }
}
