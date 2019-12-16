namespace MassTransit.AmazonSqsTransport
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using Topology;


    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + "}")]
    public readonly struct AmazonSqsEndpointAddress
    {
        const string AutoDeleteKey = "autodelete";

        public readonly string Scheme;
        public readonly string Host;
        public readonly string Scope;
        public readonly string VirtualHost;

        public readonly string Name;
        public readonly bool AutoDelete;

        public AmazonSqsEndpointAddress(Uri hostAddress, Uri address)
        {
            Scheme = default;
            Host = default;
            Scope = default;
            VirtualHost = default;

            AutoDelete = default;

            var scheme = address.Scheme.ToLowerInvariant();
            switch (scheme)
            {
                case AmazonSqsHostAddress.AmazonSqsScheme:
                    Scheme = address.Scheme;
                    Host = address.Host;

                    ParsePath(address.AbsolutePath, out Scope, out Name, out VirtualHost);
                    break;

                case "queue":
                    ParseLeft(hostAddress, out Scheme, out Host, out Scope, out VirtualHost);

                    Name = address.AbsolutePath;
                    break;

                default:
                    throw new ArgumentException($"The address scheme is not supported: {address.Scheme}", nameof(address));
            }

            AmazonSqsEntityNameValidator.Validator.ThrowIfInvalidEntityName(Name);

            var query = address.Query?.TrimStart('?');
            if (string.IsNullOrWhiteSpace(query))
                return;

            IEnumerable<(string, string)> parameters =
                query.Split('&').Select(x => x.Split('=')).Select(x => (x.First().ToLowerInvariant(), x.Skip(1).FirstOrDefault()));

            foreach (var (key, value) in parameters)
            {
                AutoDelete = key switch
                {
                    AutoDeleteKey when bool.TryParse(value, out var result) => result,
                    _ => AutoDelete
                };
            }
        }

        public AmazonSqsEndpointAddress(Uri hostAddress, string name, bool autoDelete = false)
        {
            ParseLeft(hostAddress, out Scheme, out Host, out Scope, out VirtualHost);

            Name = name;

            AutoDelete = autoDelete;
        }

        public string Path => Scope == "/" ? Name : $"{Scope}/{Name}";

        static void ParsePath(string path, out string scope, out string name, out string virtualHost)
        {
            string[] split = path.Split(AmazonSqsHostAddress.PathSeparators, StringSplitOptions.RemoveEmptyEntries);
            var length = split.Length;
            if (length > 2)
            {
                name = split[length - 1];
                virtualHost = split[length - 2];
                scope = split[length - 3];
            }
            else if (length > 1)
            {
                virtualHost = "/";
                scope = split[length - 2];
                name = split[length - 1];
            }
            else
            {
                scope = "/";
                virtualHost = "/";
                name = split[0];
            }
        }

        static void ParseLeft(Uri address, out string scheme, out string host, out string scope, out string virtualHost)
        {
            var hostAddress = new AmazonSqsHostAddress(address);
            scheme = hostAddress.Scheme;
            host = hostAddress.Host;
            scope = hostAddress.Scope;
            virtualHost = hostAddress.VirtualHost;
        }

        public static implicit operator Uri(in AmazonSqsEndpointAddress address)
        {
            var builder = new UriBuilder
            {
                Scheme = address.Scheme,
                Host = address.Host,
                Path = FormatPath(address)
            };

            builder.Query += string.Join("&", address.GetQueryStringOptions());

            return builder.Uri;
        }

        static string FormatPath(AmazonSqsEndpointAddress address)
        {
            var sb = new StringBuilder();
            if (address.Scope != "/")
                sb.Append("/").Append(address.Scope);
            if (address.VirtualHost != "/")
                sb.Append("/").Append(address.VirtualHost);
            sb.Append("/").Append(address.Name);
            return sb.ToString();
        }

        Uri DebuggerDisplay => this;

        IEnumerable<string> GetQueryStringOptions()
        {
            if (AutoDelete)
                yield return $"{AutoDeleteKey}=true";
        }
    }
}
