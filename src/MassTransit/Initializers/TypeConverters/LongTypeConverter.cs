namespace MassTransit.Initializers.TypeConverters
{
    using System;


    public class LongTypeConverter :
        ITypeConverter<string, long>,
        ITypeConverter<long, string>,
        ITypeConverter<long, object>,
        ITypeConverter<long, sbyte>,
        ITypeConverter<long, byte>,
        ITypeConverter<long, short>,
        ITypeConverter<long, ushort>,
        ITypeConverter<long, int>,
        ITypeConverter<long, uint>,
        ITypeConverter<long, ulong>
    {
        public bool TryConvert(string input, out long result)
        {
            return long.TryParse(input, out result);
        }

        public bool TryConvert(sbyte input, out long result)
        {
            result = input;
            return true;
        }

        public bool TryConvert(byte input, out long result)
        {
            result = input;
            return true;
        }

        public bool TryConvert(short input, out long result)
        {
            result = input;
            return true;
        }

        public bool TryConvert(ushort input, out long result)
        {
            result = input;
            return true;
        }

        public bool TryConvert(int input, out long result)
        {
            result = input;
            return true;
        }

        public bool TryConvert(uint input, out long result)
        {
            result = input;
            return true;
        }

        public bool TryConvert(ulong input, out long result)
        {
            result = Convert.ToInt64(input);
            return true;
        }

        public bool TryConvert(long input, out string result)
        {
            result = input.ToString();
            return true;
        }

        public bool TryConvert(object input, out long result)
        {
            if (input != null)
            {
                result = Convert.ToInt64(input);
                return true;
            }

            result = default;
            return false;
        }

    }
}
