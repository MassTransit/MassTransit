namespace MassTransit.Initializers.TypeConverters
{
    using System;


    public class BooleanTypeConverter :
        ITypeConverter<string, bool>,
        ITypeConverter<bool, string>,
        ITypeConverter<bool, object>,
        ITypeConverter<bool, sbyte>,
        ITypeConverter<bool, byte>,
        ITypeConverter<bool, short>,
        ITypeConverter<bool, ushort>,
        ITypeConverter<bool, int>,
        ITypeConverter<bool, uint>,
        ITypeConverter<bool, long>,
        ITypeConverter<bool, ulong>
    {
        public bool TryConvert(string input, out bool result)
        {
            return bool.TryParse(input, out result);
        }

        public bool TryConvert(sbyte input, out bool result)
        {
            result = Convert.ToBoolean(input);
            return true;
        }

        public bool TryConvert(byte input, out bool result)
        {
            result = Convert.ToBoolean(input);
            return true;
        }

        public bool TryConvert(short input, out bool result)
        {
            result = Convert.ToBoolean(input);
            return true;
        }

        public bool TryConvert(ushort input, out bool result)
        {
            result = Convert.ToBoolean(input);
            return true;
        }

        public bool TryConvert(int input, out bool result)
        {
            result = Convert.ToBoolean(input);
            return true;
        }

        public bool TryConvert(uint input, out bool result)
        {
            result = Convert.ToBoolean(input);
            return true;
        }

        public bool TryConvert(long input, out bool result)
        {
            result = Convert.ToBoolean(input);
            return true;
        }

        public bool TryConvert(ulong input, out bool result)
        {
            result = Convert.ToBoolean(input);
            return true;
        }

        public bool TryConvert(bool input, out string result)
        {
            result = input.ToString();
            return true;
        }

        public bool TryConvert(object input, out bool result)
        {
            if (input != null)
            {
                result = Convert.ToBoolean(input);
                return true;
            }

            result = default;
            return false;
        }
    }
}
