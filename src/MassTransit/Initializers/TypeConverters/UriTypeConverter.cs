namespace MassTransit.Initializers.TypeConverters
{
    using System;


    public class UriTypeConverter :
        ITypeConverter<string, Uri>,
        ITypeConverter<Uri, string>
    {
        public bool TryConvert(string input, out Uri result)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                result = null;
                return true;
            }

            try
            {
                result = new Uri(input);

                return true;
            }
            catch (FormatException)
            {
                result = default;
                return false;
            }
        }

        public bool TryConvert(Uri input, out string result)
        {
            result = input?.ToString();

            return true;
        }
    }
}
