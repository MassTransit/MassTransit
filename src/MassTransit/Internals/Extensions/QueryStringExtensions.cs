namespace MassTransit.Internals.Extensions
{
    using System;
    using System.Linq;


    public static class QueryStringExtensions
    {
        public static bool TryGetValueFromQueryString(this Uri uri, string key, out string value)
        {
            var queryString = uri.Query;
            if (string.IsNullOrEmpty(queryString) || queryString.Length <= 1)
            {
                value = null;
                return false;
            }

            value = queryString.Substring(1)
                .Split('&')
                .Select(x =>
                {
                    string[] values = x.Split('=');
                    if (values.Length == 2)
                    {
                        return new
                        {
                            Key = values[0],
                            Value = values[1]
                        };
                    }

                    return new
                    {
                        Key = values[0],
                        Value = ""
                    };
                })
                .Where(x => string.Compare(x.Key, key, StringComparison.OrdinalIgnoreCase) == 0)
                .Select(x => x.Value)
                .DefaultIfEmpty(null)
                .SingleOrDefault();

            return value != null;
        }

        public static T GetValueFromQueryString<T>(this Uri uri, string key, T defaultValue)
            where T : struct
        {
            if (string.IsNullOrEmpty(uri.Query))
                return defaultValue;

            try
            {
                if (!uri.TryGetValueFromQueryString(key, out var value) || string.IsNullOrEmpty(value))
                    return defaultValue;

                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}
