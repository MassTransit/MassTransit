namespace MassTransit.Initializers.TypeConverters
{
    using System;


    public class DateTimeOffsetTypeConverter :
        ITypeConverter<string, DateTimeOffset>,
        ITypeConverter<DateTimeOffset, string>
    {
        public bool TryConvert(DateTimeOffset input, out string result)
        {
            result = input.ToString("O");
            return true;
        }

        public bool TryConvert(string input, out DateTimeOffset result)
        {
            return DateTimeOffset.TryParse(input, out result);
        }
    }
}
