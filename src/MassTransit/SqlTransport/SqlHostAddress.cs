#nullable enable
namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Internals;


    /// <summary>
    /// The database host address is composed of specific parts
    /// db://localhost/virtual_host_name.scope
    /// db://localhost/.scope
    /// <list type="table">
    /// <listheader>
    /// <term>Fragment</term>
    /// <description>Description</description>
    /// </listheader>
    /// <item>
    /// <term>Host</term>
    /// <description>The host name from the connection string, or the host alias if configured</description>
    /// </item>
    /// <item>
    /// <term>Virtual Host</term>
    /// <description>
    /// The name for an isolated set of topics, queues, and subscriptions in the host/schema specified by the connection string.
    /// If not specified, the default virtual host is used.
    /// </description>
    /// </item>
    /// <item>
    /// <term>Area</term>
    /// <description>The name an area, which contains one or more queues. If not specified, the default area within the virtual host is used.</description>
    /// </item>
    /// </list>
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + "}")]
    public readonly struct SqlHostAddress
    {
        public const string DbScheme = "db";

        const string InstanceNameKey = "instance";

        public readonly string Scheme;
        public readonly string Host;
        public readonly int? Port;
        public readonly string? InstanceName;
        public readonly string VirtualHost;
        public readonly string? Area;

        public SqlHostAddress(Uri address)
        {
            var scheme = address.Scheme.ToLowerInvariant();
            switch (scheme)
            {
                case DbScheme:
                    ParseLeft(address, out Scheme, out Host, out Port, out VirtualHost, out Area);
                    break;

                default:
                    throw new ArgumentException($"The address scheme is not supported: {address.Scheme}", nameof(address));
            }

            foreach (var (key, value) in address.SplitQueryString())
            {
                switch (key)
                {
                    case InstanceNameKey when !string.IsNullOrWhiteSpace(value):
                        InstanceName = value;
                        break;
                }
            }
        }

        public SqlHostAddress(string host, string? instanceName, int? port, string virtualHost, string? area)
        {
            Scheme = DbScheme;
            Host = host;
            InstanceName = instanceName;
            Port = port;
            VirtualHost = virtualHost;
            Area = area;
        }

        internal static void ParseLeft(Uri address, out string scheme, out string host, out int? port, out string virtualHost, out string? area)
        {
            scheme = address.Scheme;
            host = address.Host;
            port = address.IsDefaultPort ? null : address.Port;

            (virtualHost, area) = GetVirtualHostAndArea(address);
        }

        static (string virtualHost, string? area) GetVirtualHostAndArea(Uri address)
        {
            var path = address.AbsolutePath;

            if (string.IsNullOrWhiteSpace(path))
                return ("/", null);

            if (path.Length == 1 && path[0] == '/')
                return ("/", null);

            var split = path.LastIndexOf('/');

            ReadOnlySpan<char> span = split > 0
                ? path.AsSpan(1, split - 1)
                : path.AsSpan(1);

            string virtualHost;
            string? area = null;

            var areaSplit = span.IndexOf('.');
            if (areaSplit > 0)
            {
                virtualHost = Uri.UnescapeDataString(span.Slice(0, areaSplit).ToString()).Trim();
                area = Uri.UnescapeDataString(span.Slice(areaSplit + 1).ToString()).Trim();
            }
            else
                virtualHost = Uri.UnescapeDataString(span.ToString()).Trim();

            if (string.IsNullOrWhiteSpace(virtualHost))
                return ("/", null);

            if (!IsValidSymbol(virtualHost))
                throw new SqlEndpointAddressException(address, "Virtual host must be alphanumeric");

            if (string.IsNullOrWhiteSpace(area))
                return (virtualHost, null);

            if (!IsValidSymbol(area))
                throw new SqlEndpointAddressException(address, "Area must be alphanumeric");

            return (virtualHost, area);
        }

        public static implicit operator Uri(in SqlHostAddress address)
        {
            var path = address.VirtualHost == "/" ? "/" : Uri.EscapeDataString(address.VirtualHost);
            if (!string.IsNullOrWhiteSpace(address.Area))
                path += "." + Uri.EscapeDataString(address.Area);

            var builder = new UriBuilder
            {
                Scheme = address.Scheme,
                Host = address.Host.Trim().Trim('(', ')'),
                Port = address.Port ?? -1,
                Path = path
            };


            builder.Query += string.Join("&", address.GetQueryStringOptions());

            return builder.Uri;
        }

        Uri DebuggerDisplay => this;

        IEnumerable<string> GetQueryStringOptions()
        {
            if (!string.IsNullOrEmpty(InstanceName))
                yield return $"{InstanceNameKey}={InstanceName}";
        }

        static bool IsValidSymbol(string? className)
        {
            if (string.IsNullOrEmpty(className))
                return false;

            var c0 = className![0];
            if (!(char.IsLetter(c0) || c0 == '_'))
                return false;

            for (var i = 1; i < className.Length; i++)
            {
                var c = className[i];
                if (!(char.IsLetterOrDigit(c) || c == '_'))
                    return false;
            }

            return true;
        }
    }
}
