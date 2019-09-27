namespace MassTransit.Initializers.TypeConverters
{
    using System;


    public class UriTypeConverter :
        ITypeConverter<string, Uri>,
        ITypeConverter<Uri, string>,
        ITypeConverter<Uri, object>
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

        public bool TryConvert(object input, out Uri result)
        {
            switch (input)
            {
                case Uri uri:
                    result = uri;
                    return true;

                case string text when !string.IsNullOrWhiteSpace(text):
                    try
                    {
                        result = new Uri(text);
                        return true;
                    }
                    catch (FormatException)
                    {
                        result = default;
                        return false;
                    }

                default:
                    result = default;
                    return false;
            }
        }
    }
}
