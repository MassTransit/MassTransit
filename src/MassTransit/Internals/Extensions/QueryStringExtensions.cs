namespace MassTransit.Internals.Extensions
{
    using System;
    using System.Globalization;
    using System.Linq;


    public static class QueryStringExtensions
    {
        public static bool TryGetValueFromQueryString(this Uri uri, string key, out string value)
        {
            string queryString = uri.Query;
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

                                       return new
                                           {
                                               Key = values[0],
                                               Value = values[1]
                                           };
                                   })
                               .Where(x => string.Compare(x.Key, key, true, CultureInfo.InvariantCulture) == 0)
                               .Select(x => x.Value)
                               .DefaultIfEmpty(null)
                               .SingleOrDefault();

            return value != null;
        }
    }
}