namespace MassTransit.AmazonSqsTransport
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Topology;


    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + "}")]
    public readonly struct AmazonSqsEndpointAddress
    {
        const string AutoDeleteKey = "autodelete";

        public readonly string Scheme;
        public readonly string Host;
        public readonly string Scope;

        public readonly string Name;
        public readonly bool AutoDelete;

        public AmazonSqsEndpointAddress(Uri hostAddress, Uri address)
        {
            Scheme = default;
            Host = default;
            Scope = default;

            AutoDelete = default;

            var scheme = address.Scheme.ToLowerInvariant();
            switch (scheme)
            {
                case AmazonSqsHostAddress.AmazonSqsScheme:
                    Scheme = address.Scheme;
                    Host = address.Host;

                    ParsePath(address.AbsolutePath, out Scope, out Name);
                    break;

                case "queue":
                    ParseLeft(hostAddress, out Scheme, out Host, out Scope);

                    Name = address.AbsolutePath;
                    break;

                default:
                    throw new ArgumentException($"The address scheme is not supported: {address.Scheme}", nameof(address));
            }

            AmazonSqsEntityNameValidator.Validator.ThrowIfInvalidEntityName(Name);

            string query = address.Query?.TrimStart('?');
            if (!string.IsNullOrWhiteSpace(query))
            {
                var parameters = query.Split('&').Select(x => x.Split('=')).Select(x => (x.First().ToLowerInvariant(), x.Skip(1).FirstOrDefault()));

                foreach ((string key, string value) in parameters)
                {
                    switch (key)
                    {
                        case AutoDeleteKey when bool.TryParse(value, out var result):
                            AutoDelete = result;
                            break;
                    }
                }
            }
        }

        public AmazonSqsEndpointAddress(Uri hostAddress, string name, bool autoDelete = false)
        {
            ParseLeft(hostAddress, out Scheme, out Host, out Scope);

            Name = name;

            AutoDelete = autoDelete;
        }

        public string Path => Scope == "/" ? Name : $"{Scope}/{Name}";

        static void ParsePath(string path, out string scope, out string name)
        {
            int split = path.LastIndexOf('/');
            if (split > 0)
            {
                scope = path.Substring(1, split - 1);
                name = path.Substring(split + 1);
            }
            else
            {
                scope = "/";
                name = path.Substring(1);
            }
        }

        static void ParseLeft(Uri address, out string scheme, out string host, out string scope)
        {
            var hostAddress = new AmazonSqsHostAddress(address);
            scheme = hostAddress.Scheme;
            host = hostAddress.Host;
            scope = hostAddress.Scope;
        }

        public static implicit operator Uri(in AmazonSqsEndpointAddress address)
        {
            var builder = new UriBuilder
            {
                Scheme = address.Scheme,
                Host = address.Host,
                Path = address.Scope == "/"
                    ? $"/{address.Name}"
                    : $"/{address.Scope}/{address.Name}"
            };

            builder.Query += string.Join("&", address.GetQueryStringOptions());

            return builder.Uri;
        }

        Uri DebuggerDisplay => this;

        IEnumerable<string> GetQueryStringOptions()
        {
            if (AutoDelete)
                yield return $"{AutoDeleteKey}=true";
        }
    }
}
