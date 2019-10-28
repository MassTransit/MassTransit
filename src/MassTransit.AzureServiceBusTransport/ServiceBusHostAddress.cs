namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Diagnostics;


    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + "}")]
    public readonly struct ServiceBusHostAddress
    {
        public readonly string Scheme;
        public readonly string Host;
        public readonly string Scope;

        public ServiceBusHostAddress(Uri address)
        {
            Scheme = default;
            Host = default;
            Scope = default;

            var scheme = address.Scheme.ToLowerInvariant();
            switch (scheme)
            {
                case "sb":
                    Scheme = address.Scheme;
                    Host = address.Host;

                    ParseLeft(address, out Scheme, out Host, out Scope);
                    break;

                default:
                    throw new ArgumentException($"The address scheme is not supported: {address.Scheme}", nameof(address));
            }
        }

        static void ParseLeft(Uri address, out string scheme, out string host, out string scope)
        {
            scheme = address.Scheme;
            host = address.Host;

            scope = ParseVirtualHost(address.AbsolutePath);
        }

        static string ParseVirtualHost(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return "/";

            if (path.Length == 1 && path[0] == '/')
                return path;

            int split = path.LastIndexOf('/');
            if (split > 0)
                return Uri.UnescapeDataString(path.Substring(1, split - 1));

            return Uri.UnescapeDataString(path.Substring(1));
        }

        public static implicit operator Uri(in ServiceBusHostAddress address)
        {
            var builder = new UriBuilder
            {
                Scheme = address.Scheme,
                Host = address.Host,
                Path = address.Scope == "/"
                    ? $"/"
                    : $"/{Uri.EscapeDataString(address.Scope)}"
            };

            return builder.Uri;
        }

        Uri DebuggerDisplay => this;
    }
}
