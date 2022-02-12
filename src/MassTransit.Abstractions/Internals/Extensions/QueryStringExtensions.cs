namespace MassTransit.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


    public static class QueryStringExtensions
    {
        public static bool TryGetValueFromQueryString(this Uri uri, string key, out string? value)
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
                    var values = x.Split('=');
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

        /// <summary>
        /// Parse the host path, which on a host address might be a virtual host, a scope, etc.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static string ParseHostPath(this Uri address)
        {
            var path = address.AbsolutePath;

            if (string.IsNullOrWhiteSpace(path))
                return "/";

            if (path.Length == 1 && path[0] == '/')
                return path;

            var split = path.LastIndexOf('/');
            if (split > 0)
                return Uri.UnescapeDataString(path.Substring(1, split - 1));

            return Uri.UnescapeDataString(path.Substring(1));
        }

        /// <summary>
        /// Parse the host path and entity name from the address
        /// </summary>
        /// <param name="address"></param>
        /// <param name="hostPath"></param>
        /// <param name="entityName"></param>
        public static void ParseHostPathAndEntityName(this Uri address, out string hostPath, out string entityName)
        {
            var path = address.AbsolutePath;

            var split = path.LastIndexOf('/');
            if (split > 0)
            {
                hostPath = Uri.UnescapeDataString(path.Substring(1, split - 1));
                entityName = path.Substring(split + 1);
            }
            else
            {
                hostPath = "/";
                entityName = path.Substring(1);
            }
        }

        /// <summary>
        /// Split the query string into an enumerable stream of tuples
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static IEnumerable<(string, string)> SplitQueryString(this Uri address)
        {
            var query = address.Query?.TrimStart('?');
            return string.IsNullOrWhiteSpace(query)
                ? Array.Empty<(string, string)>()
                : query!.Split('&').Select(x => x.Split('=')).Select(x => (x.First().ToLowerInvariant(), x.Skip(1).FirstOrDefault()));
        }
    }
}
