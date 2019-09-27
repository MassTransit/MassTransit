namespace MassTransit.Initializers.TypeConverters
{
    public class StringTypeConverter :
        ITypeConverter<string, object>
    {
        public bool TryConvert(object input, out string result)
        {
            if (input != null)
            {
                result = input.ToString();
                return true;
            }

            result = null;
            return false;
        }
    }
}