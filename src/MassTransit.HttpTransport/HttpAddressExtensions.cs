namespace MassTransit.HttpTransport
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using Clients;
    using Hosting;


    public static class HttpAddressExtensions
    {
        public static HttpSendSettings GetSendSettings(this Uri uri)
        {
            return new HttpSendSettingsImpl(HttpMethod.Post, uri.LocalPath);
        }

        public static HttpHostSettings GetHostSettings(this Uri uri)
        {
            return new ConfigurationHostSettings
            {
                Scheme = uri.Scheme,
                Host = uri.Host,
                Port = uri.Port,
                Method = HttpMethod.Get
            };
        }

        public static Uri GetInputAddress(this HttpHostSettings hostSettings)
        {
            var builder = new UriBuilder
            {
                Scheme = hostSettings.Scheme,
                Host = hostSettings.Host,
                Port = hostSettings.Port,
            };

            return builder.Uri;
        }

        static IEnumerable<string> GetQueryStringOptions(HttpSendSettings settings)
        {
            return Enumerable.Empty<string>();
        }

        public static Uri GetSendAddress(this HttpHostSettings hostSettings, HttpSendSettings sendSettings)
        {
            var builder = new UriBuilder
            {
                Scheme = "http",
                Host = hostSettings.Host,
                Port = hostSettings.Port,
                Path = sendSettings.Path
            };

            builder.Query += string.Join("&", GetQueryStringOptions(sendSettings));

            return builder.Uri;
        }
    }
}
