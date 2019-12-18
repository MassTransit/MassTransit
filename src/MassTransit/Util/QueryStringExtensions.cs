namespace MassTransit.Util
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


    public static class QueryStringExtensions
    {
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

            int split = path.LastIndexOf('/');
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

            int split = path.LastIndexOf('/');
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
            string query = address.Query?.TrimStart('?');
            if (!string.IsNullOrWhiteSpace(query))
            {
                return query.Split('&').Select(x => x.Split('=')).Select(x => (x.First().ToLowerInvariant(), x.Skip(1).FirstOrDefault()));
            }

            return Enumerable.Empty<(string, string)>();
        }
    }
}
