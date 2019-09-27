namespace MassTransit.Initializers.TypeConverters
{
    using System;
    using System.Globalization;


    public class DoubleTypeConverter :
        ITypeConverter<string, double>,
        ITypeConverter<double, string>,
        ITypeConverter<double, object>,
        ITypeConverter<double, sbyte>,
        ITypeConverter<double, byte>,
        ITypeConverter<double, short>,
        ITypeConverter<double, ushort>,
        ITypeConverter<double, int>,
        ITypeConverter<double, uint>,
        ITypeConverter<double, long>,
        ITypeConverter<double, ulong>
    {
        public bool TryConvert(string input, out double result)
        {
            return double.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
        }

        public bool TryConvert(sbyte input, out double result)
        {
            result = Convert.ToDouble(input);
            return true;
        }

        public bool TryConvert(byte input, out double result)
        {
            result = Convert.ToDouble(input);
            return true;
        }

        public bool TryConvert(short input, out double result)
        {
            result = Convert.ToDouble(input);
            return true;
        }

        public bool TryConvert(ushort input, out double result)
        {
            result = Convert.ToDouble(input);
            return true;
        }

        public bool TryConvert(int input, out double result)
        {
            result = Convert.ToDouble(input);
            return true;
        }

        public bool TryConvert(uint input, out double result)
        {
            result = Convert.ToDouble(input);
            return true;
        }

        public bool TryConvert(long input, out double result)
        {
            result = Convert.ToDouble(input);
            return true;
        }

        public bool TryConvert(ulong input, out double result)
        {
            result = Convert.ToDouble(input);
            return true;
        }

        public bool TryConvert(double input, out string result)
        {
            result = input.ToString(CultureInfo.InvariantCulture);
            return true;
        }

        public bool TryConvert(object input, out double result)
        {
            if (input != null)
            {
                result = Convert.ToDouble(input);
                return true;
            }

            result = default;
            return false;
        }

    }
}
