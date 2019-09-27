namespace MassTransit.Initializers.TypeConverters
{
    using System;


    public class IntTypeConverter :
        ITypeConverter<string, int>,
        ITypeConverter<int, object>,
        ITypeConverter<int, string>,
        ITypeConverter<int, sbyte>,
        ITypeConverter<int, byte>,
        ITypeConverter<int, short>,
        ITypeConverter<int, ushort>,
        ITypeConverter<int, uint>,
        ITypeConverter<int, long>,
        ITypeConverter<int, ulong>
    {
        public bool TryConvert(string input, out int result)
        {
            return int.TryParse(input, out result);
        }

        public bool TryConvert(byte input, out int result)
        {
            result = input;
            return true;
        }

        public bool TryConvert(short input, out int result)
        {
            result = input;
            return true;
        }

        public bool TryConvert(sbyte input, out int result)
        {
            result = input;
            return true;
        }

        public bool TryConvert(ushort input, out int result)
        {
            result = input;
            return true;
        }

        public bool TryConvert(uint input, out int result)
        {
            result = Convert.ToInt32(input);
            return true;
        }

        public bool TryConvert(long input, out int result)
        {
            result = Convert.ToInt32(input);
            return true;
        }

        public bool TryConvert(ulong input, out int result)
        {
            result = Convert.ToInt32(input);
            return true;
        }

        public bool TryConvert(int input, out string result)
        {
            result = input.ToString();
            return true;
        }

        public bool TryConvert(object input, out int result)
        {
            if (input != null)
            {
                result = Convert.ToInt32(input);
                return true;
            }

            result = default;
            return false;
        }
    }
}
