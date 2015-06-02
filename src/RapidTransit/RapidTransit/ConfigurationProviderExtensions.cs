namespace RapidTransit
{
    using Configuration;


    public static class ConfigurationProviderExtensions
    {
        public static string GetSetting(this IConfigurationProvider configuration, string key, string defaultValue)
        {
            string value;
            return configuration.TryGetSetting(key, out value)
                       ? (string.IsNullOrEmpty(value) ? defaultValue : value)
                       : defaultValue;
        }

        public static int GetSetting(this IConfigurationProvider configuration, string key, int defaultValue)
        {
            string value;
            int result;
            return configuration.TryGetSetting(key, out value)
                       ? (string.IsNullOrEmpty(value)
                              ? defaultValue
                              : int.TryParse(value, out result) ? result : defaultValue)
                       : defaultValue;
        }

        public static long GetSetting(this IConfigurationProvider configuration, string key, long defaultValue)
        {
            string value;
            long result;
            return configuration.TryGetSetting(key, out value)
                       ? (string.IsNullOrEmpty(value)
                              ? defaultValue
                              : long.TryParse(value, out result) ? result : defaultValue)
                       : defaultValue;
        }

        public static ushort GetSetting(this IConfigurationProvider configuration, string key, ushort defaultValue)
        {
            string value;
            ushort result;
            return configuration.TryGetSetting(key, out value)
                       ? (string.IsNullOrEmpty(value)
                              ? defaultValue
                              : ushort.TryParse(value, out result) ? result : defaultValue)
                       : defaultValue;
        }

        public static bool GetSetting(this IConfigurationProvider configuration, string key, bool defaultValue)
        {
            string value;
            bool result;
            return configuration.TryGetSetting(key, out value)
                       ? (string.IsNullOrEmpty(value)
                              ? defaultValue
                              : bool.TryParse(value, out result) ? result : defaultValue)
                       : defaultValue;
        }

        public static decimal GetSetting(this IConfigurationProvider configuration, string key, decimal defaultValue)
        {
            string value;
            decimal result;
            return configuration.TryGetSetting(key, out value)
                       ? (string.IsNullOrEmpty(value)
                              ? defaultValue
                              : decimal.TryParse(value, out result) ? result : defaultValue)
                       : defaultValue;
        }

        public static double GetSetting(this IConfigurationProvider configuration, string key, double defaultValue)
        {
            string value;
            double result;
            return configuration.TryGetSetting(key, out value)
                       ? (string.IsNullOrEmpty(value)
                              ? defaultValue
                              : double.TryParse(value, out result) ? result : defaultValue)
                       : defaultValue;
        }

        public static uint GetSetting(this IConfigurationProvider configuration, string key, uint defaultValue)
        {
            string value;
            uint result;
            return configuration.TryGetSetting(key, out value)
                       ? (string.IsNullOrEmpty(value)
                              ? defaultValue
                              : uint.TryParse(value, out result) ? result : defaultValue)
                       : defaultValue;
        }
    }
}