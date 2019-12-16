namespace MassTransit.AmazonSqsTransport
{
    using System;
    using System.Diagnostics;
    using System.Text;


    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + "}")]
    public readonly struct AmazonSqsHostAddress
    {
        public static readonly string[] PathSeparators = {"/"};
        public const string AmazonSqsScheme = "amazonsqs";

        public readonly string Scheme;
        public readonly string Host;
        public readonly string Scope;
        public readonly string VirtualHost;

        public AmazonSqsHostAddress(Uri address)
        {
            Scheme = default;
            Host = default;
            Scope = default;
            VirtualHost = default;

            var scheme = address.Scheme.ToLowerInvariant();
            switch (scheme)
            {
                case AmazonSqsScheme:
                    Scheme = address.Scheme;
                    Host = address.Host;

                    ParseLeft(address, out Scheme, out Host, out Scope, out VirtualHost);
                    break;

                default:
                    throw new ArgumentException($"The address scheme is not supported: {address.Scheme}", nameof(address));
            }
        }

        public AmazonSqsHostAddress(string host, string scope, string virtualHost)
        {
            Scheme = AmazonSqsScheme;
            Host = host;
            Scope = scope;
            VirtualHost = virtualHost;
        }

        static void ParseLeft(Uri address, out string scheme, out string host, out string scope, out string virtualHost)
        {
            scheme = address.Scheme;
            host = address.Host;

            string[] split = address.AbsolutePath.Split(PathSeparators, StringSplitOptions.RemoveEmptyEntries);
            var length = split.Length;
            if (length > 1)
            {
                virtualHost = Uri.UnescapeDataString(split[length - 1]);
                scope = Uri.UnescapeDataString(split[length - 2]);
            }
            else if (length > 0)
            {
                scope = Uri.UnescapeDataString(split[length - 1]);
                virtualHost = "/";
            }
            else
            {
                scope = "/";
                virtualHost = "/";
            }
        }

        public static implicit operator Uri(in AmazonSqsHostAddress address)
        {
            var builder = new UriBuilder
            {
                Scheme = address.Scheme,
                Host = address.Host,
                Path = FormatPath(address)
            };

            return builder.Uri;
        }

        static string FormatPath(AmazonSqsHostAddress address)
        {
            var sb = new StringBuilder();
            if (address.Scope != "/")
                sb.Append("/").Append(address.Scope);
            if (address.VirtualHost != "/")
                sb.Append("/").Append(address.VirtualHost);
            return sb.Length > 0 ? sb.ToString() : "/";
        }

        Uri DebuggerDisplay => this;
    }
}
