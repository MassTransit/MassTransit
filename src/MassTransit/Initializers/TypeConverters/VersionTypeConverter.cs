namespace MassTransit.Initializers.TypeConverters
{
    using System;


    public class VersionTypeConverter :
        ITypeConverter<string, Version>,
        ITypeConverter<Version, string>
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
    }
}
