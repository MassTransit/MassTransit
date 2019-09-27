namespace MassTransit.Initializers.TypeConverters
{
    using System;


    public class VersionTypeConverter :
        ITypeConverter<string, Version>,
        ITypeConverter<Version, string>,
        ITypeConverter<Version, object>
    {
        public bool TryConvert(string input, out Version result)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                result = null;
                return true;
            }

            try
            {
                result = new Version(input);

                return true;
            }
            catch (Exception)
            {
                result = default;
                return false;
            }
        }

        public bool TryConvert(Version input, out string result)
        {
            result = input?.ToString();

            return true;
        }

        public bool TryConvert(object input, out Version result)
        {
            switch (input)
            {
                case Version version:
                    result = version;
                    return true;

                case string text when !string.IsNullOrWhiteSpace(text):
                    try
                    {
                        result = new Version(text);
                        return true;
                    }
                    catch (Exception)
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
