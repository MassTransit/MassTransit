namespace RapidTransit
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;


    public static class RabbitMqSettingsExtensions
    {
        static readonly Regex _prefetch = new Regex(@"([\?\&])prefetch=[^\&]+[\&]?", RegexOptions.Compiled);

        public static Uri GetQueueAddress(this RabbitMqSettings settings, string queueName,
            int? consumerLimit = default(int?))
        {
            string[] paths = settings.VirtualHost.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
            string path = string.Join("/", paths.Concat(new[] {queueName}).ToArray());

            var builder = new UriBuilder("rabbitmq", settings.Host, settings.Port)
                {
                    Path = path,
                    Query = settings.Options
                };

            if (consumerLimit.HasValue)
                return SetPrefetchCount(builder.Uri, consumerLimit.Value);

            return builder.Uri;
        }

        static Uri SetPrefetchCount(Uri uri, int consumerLimit)
        {
            string query = uri.Query;

            if (query.IndexOf("prefetch", StringComparison.InvariantCultureIgnoreCase) >= 0)
                query = _prefetch.Replace(query, string.Format("prefetch={0}", consumerLimit));
            else if (string.IsNullOrEmpty(query))
                query = string.Format("prefetch={0}", consumerLimit);
            else
                query += string.Format("&prefetch={0}", consumerLimit);

            var builder = new UriBuilder(uri)
                {
                    Query = query.Trim('?')
                };

            return builder.Uri;
        }
    }
}