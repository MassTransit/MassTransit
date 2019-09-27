namespace MassTransit.Initializers.TypeConverters
{
    using System;


    public class ShortTypeConverter :
        ITypeConverter<string, short>,
        ITypeConverter<short, string>,
        ITypeConverter<short, object>,
        ITypeConverter<short, sbyte>,
        ITypeConverter<short, byte>,
        ITypeConverter<short, ushort>,
        ITypeConverter<short, int>,
        ITypeConverter<short, uint>,
        ITypeConverter<short, long>,
        ITypeConverter<short, ulong>
    {
        public bool TryConvert(string input, out short result)
        {
            return short.TryParse(input, out result);
        }

        public bool TryConvert(sbyte input, out short result)
        {
            result = input;
            return true;
        }

        public bool TryConvert(byte input, out short result)
        {
            result = input;
            return true;
        }

        public bool TryConvert(ushort input, out short result)
        {
            result = Convert.ToInt16(input);
            return true;
        }

        public bool TryConvert(int input, out short result)
        {
            result = Convert.ToInt16(input);
            return true;
        }

        public bool TryConvert(uint input, out short result)
        {
            result = Convert.ToInt16(input);
            return true;
        }

        public bool TryConvert(long input, out short result)
        {
            result = Convert.ToInt16(input);
            return true;
        }

        public bool TryConvert(ulong input, out short result)
        {
            result = Convert.ToInt16(input);
            return true;
        }

        public bool TryConvert(short input, out string result)
        {
            result = input.ToString();
            return true;
        }

        public bool TryConvert(object input, out short result)
        {
            if (input != null)
            {
                result = Convert.ToInt16(input);
                return true;
            }

            result = default;
            return false;
        }
    }
}
