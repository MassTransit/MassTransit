namespace MassTransit.Initializers.TypeConverters
{
    using System;


    public class ByteTypeConverter :
        ITypeConverter<string, byte>,
        ITypeConverter<byte, string>,
        ITypeConverter<byte, object>,
        ITypeConverter<byte, sbyte>,
        ITypeConverter<byte, short>,
        ITypeConverter<byte, ushort>,
        ITypeConverter<byte, int>,
        ITypeConverter<byte, uint>,
        ITypeConverter<byte, long>,
        ITypeConverter<byte, ulong>
    {
        public bool TryConvert(string input, out byte result)
        {
            return byte.TryParse(input, out result);
        }

        public bool TryConvert(sbyte input, out byte result)
        {
            result = Convert.ToByte(input);
            return true;
        }

        public bool TryConvert(short input, out byte result)
        {
            result = Convert.ToByte(input);
            return true;
        }

        public bool TryConvert(ushort input, out byte result)
        {
            result = Convert.ToByte(input);
            return true;
        }

        public bool TryConvert(int input, out byte result)
        {
            result = Convert.ToByte(input);
            return true;
        }

        public bool TryConvert(uint input, out byte result)
        {
            result = Convert.ToByte(input);
            return true;
        }

        public bool TryConvert(long input, out byte result)
        {
            result = Convert.ToByte(input);
            return true;
        }

        public bool TryConvert(ulong input, out byte result)
        {
            result = Convert.ToByte(input);
            return true;
        }

        public bool TryConvert(byte input, out string result)
        {
            result = input.ToString();
            return true;
        }

        public bool TryConvert(object input, out byte result)
        {
            if (input != null)
            {
                result = Convert.ToByte(input);
                return true;
            }

            result = default;
            return false;
        }
    }
}
