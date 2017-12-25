// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Text.RegularExpressions;
    using Configurators;
    using NewIdFormatters;
    using RabbitMQ.Client;
    using Topology;
    using Transport;
    using Util;


    public static class RabbitMqAddressExtensions
    {
        static readonly INewIdFormatter _formatter = new ZBase32Formatter();
        static readonly Regex _regex = new Regex(@"^[A-Za-z0-9\-_\.:]+$");

        public static string GetTemporaryQueueName(this IRabbitMqHost ignored, string prefix)
        {
            var sb = new StringBuilder(prefix);

            var host = HostMetadataCache.Host;

            foreach (char c in host.MachineName)
            {
                if (char.IsLetterOrDigit(c))
                    sb.Append(c);
                else if (c == '.' || c == '_' || c == '-' || c == ':')
                    sb.Append(c);
            }
            sb.Append('-');
            foreach (char c in host.ProcessName)
            {
                if (char.IsLetterOrDigit(c))
                    sb.Append(c);
                else if (c == '.' || c == '_' || c == '-' || c == ':')
                    sb.Append(c);
            }
            sb.Append('-');
            sb.Append(NewId.Next().ToString(_formatter));

            return sb.ToString();
        }

        public static ReceiveSettings GetReceiveSettings(this Uri address)
        {
            if (string.Compare("rabbitmq", address.Scheme, StringComparison.OrdinalIgnoreCase) != 0)
                throw new RabbitMqAddressException("The invalid scheme was specified: " + address.Scheme);

            var connectionFactory = new ConnectionFactory
            {
                HostName = address.Host,
                UserName = "guest",
                Password = "guest",
            };

            if (address.IsDefaultPort)
                connectionFactory.Port = 5672;
            else if (!address.IsDefaultPort)
                connectionFactory.Port = address.Port;

            string name = address.AbsolutePath.Substring(1);
            string[] pathSegments = name.Split('/');
            if (pathSegments.Length == 2)
            {
                connectionFactory.VirtualHost = pathSegments[0];
                name = pathSegments[1];
            }

            ushort heartbeat = address.Query.GetValueFromQueryString("heartbeat", connectionFactory.RequestedHeartbeat);
            connectionFactory.RequestedHeartbeat = heartbeat;

            if (name == "*")
            {
                string uri = address.GetComponents(UriComponents.Scheme | UriComponents.StrongAuthority | UriComponents.Path, UriFormat.UriEscaped);
                if (uri.EndsWith("*"))
                {
                    name = NewId.Next().ToString("NS");
                    uri = uri.Remove(uri.Length - 1) + name;

                    var builder = new UriBuilder(uri)
                    {
                        Query = string.IsNullOrEmpty(address.Query) ? "" : address.Query.Substring(1)
                    };

                    address = builder.Uri;
                }
                else
                    throw new InvalidOperationException("Uri is not properly formed");
            }
            else
                VerifyQueueOrExchangeNameIsLegal(name);

            ushort prefetch = address.Query.GetValueFromQueryString("prefetch", (ushort)Math.Max(Environment.ProcessorCount, 16));
            int timeToLive = address.Query.GetValueFromQueryString("ttl", 0);

            bool isTemporary = address.Query.GetValueFromQueryString("temporary", false);

            bool durable = address.Query.GetValueFromQueryString("durable", !isTemporary);
            bool exclusive = address.Query.GetValueFromQueryString("exclusive", isTemporary);
            bool autoDelete = address.Query.GetValueFromQueryString("autodelete", isTemporary);

            ReceiveSettings settings = new RabbitMqReceiveSettings(name, ExchangeType.Fanout, durable, autoDelete)
            {
                Exclusive = exclusive,
                QueueName = name,
                PrefetchCount = prefetch,
            };

            if (timeToLive > 0)
                settings.QueueArguments.Add("x-message-ttl", timeToLive.ToString("F0", CultureInfo.InvariantCulture));

            return settings;
        }

        public static ConnectionFactory GetConnectionFactory(this RabbitMqHostSettings settings)
        {
            var factory = new ConnectionFactory
            {
                AutomaticRecoveryEnabled = false,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(1),
                TopologyRecoveryEnabled = false,
                HostName = settings.Host,
                Port = settings.Port,
                VirtualHost = settings.VirtualHost ?? "/",
                RequestedHeartbeat = settings.Heartbeat,
                RequestedConnectionTimeout = 10000
            };

            if (settings.ClusterMembers != null && settings.ClusterMembers.Any())
            {
                factory.HostName = null;
                factory.EndpointResolverFactory = x => new SequentialEndpointResolver(settings.ClusterMembers);
            }
            
            if (settings.UseClientCertificateAsAuthenticationIdentity)
            {
                factory.AuthMechanisms.Clear();
                factory.AuthMechanisms.Add(new ExternalMechanismFactory());
                factory.UserName = "";
                factory.Password = "";
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(settings.Username))
                    factory.UserName = settings.Username;
                if (!string.IsNullOrWhiteSpace(settings.Password))
                    factory.Password = settings.Password;
            }

            factory.Ssl.Enabled = settings.Ssl;
            factory.Ssl.Version = settings.SslProtocol;
            factory.Ssl.AcceptablePolicyErrors = settings.AcceptablePolicyErrors;
            factory.Ssl.ServerName = settings.SslServerName;
            factory.Ssl.Certs = settings.ClientCertificate == null ? null : new X509Certificate2Collection {settings.ClientCertificate};
            factory.Ssl.CertificateSelectionCallback = settings.CertificateSelectionCallback;
            factory.Ssl.CertificateValidationCallback = settings.CertificateValidationCallback;

            if (string.IsNullOrWhiteSpace(factory.Ssl.ServerName))
                factory.Ssl.AcceptablePolicyErrors |= SslPolicyErrors.RemoteCertificateNameMismatch;

            if (string.IsNullOrEmpty(settings.ClientCertificatePath))
            {
                factory.Ssl.CertPath = "";
                factory.Ssl.CertPassphrase = "";
            }
            else
            {
                factory.Ssl.CertPath = settings.ClientCertificatePath;
                factory.Ssl.CertPassphrase = settings.ClientCertificatePassphrase;
            }
            
            factory.ClientProperties = factory.ClientProperties ?? new Dictionary<string, object>();

            HostInfo hostInfo = HostMetadataCache.Host;

            factory.ClientProperties["client_api"] = "MassTransit";
            factory.ClientProperties["masstransit_version"] = hostInfo.MassTransitVersion;
            factory.ClientProperties["net_version"] = hostInfo.FrameworkVersion;
            factory.ClientProperties["hostname"] = hostInfo.MachineName;
            factory.ClientProperties["connected"] = DateTimeOffset.Now.ToString("R");
            factory.ClientProperties["process_id"] = hostInfo.ProcessId.ToString();
            factory.ClientProperties["process_name"] = hostInfo.ProcessName;
            if (hostInfo.Assembly != null)
                factory.ClientProperties["assembly"] = hostInfo.Assembly;
            if (hostInfo.AssemblyVersion != null)
                factory.ClientProperties["assembly_version"] = hostInfo.AssemblyVersion;

            if (string.IsNullOrEmpty(settings.ClientProvidedName))
            {
                factory.ClientProperties["connection_name"] = $"{hostInfo.MachineName}.{hostInfo.Assembly}_{hostInfo.ProcessName}";
            }
            else
            {
                factory.ClientProperties["connection_name"] = settings.ClientProvidedName;
            }

            return factory;
        }

        public static RabbitMqHostSettings GetHostSettings(this Uri address)
        {
            return GetConfigurationHostSettings(address);
        }

        internal static ConfigurationHostSettings GetConfigurationHostSettings(this Uri address)
        {
            if (string.Compare("rabbitmq", address.Scheme, StringComparison.OrdinalIgnoreCase) != 0)
                throw new RabbitMqAddressException("The invalid scheme was specified: " + address.Scheme);

            var hostSettings = new ConfigurationHostSettings
            {
                Host = address.Host,
                Username = "",
                Password = "",
                Port = address.IsDefaultPort ? 5672 : address.Port
            };

            if (!string.IsNullOrEmpty(address.UserInfo))
            {
                string[] parts = address.UserInfo.Split(':');
                hostSettings.Username = parts[0];

                if (parts.Length >= 2)
                    hostSettings.Password = parts[1];
            }

            string name = address.AbsolutePath.Substring(1);

            string[] pathSegments = name.Split('/');
            hostSettings.VirtualHost = pathSegments.Length == 2 ? pathSegments[0] : "/";

            hostSettings.Heartbeat = address.Query.GetValueFromQueryString("heartbeat", (ushort)0);

            return hostSettings;
        }

        static void VerifyQueueOrExchangeNameIsLegal(string queueName)
        {
            if (string.IsNullOrWhiteSpace(queueName))
                throw new RabbitMqAddressException("The queue name must not be null or empty");

            bool success = IsValidQueueName(queueName);
            if (!success)
            {
                throw new RabbitMqAddressException(
                    "The queueName must be a sequence of these characters: letters, digits, hyphen, underscore, period, or colon.");
            }
        }

        public static bool IsValidQueueName(string queueName)
        {
            Match match = _regex.Match(queueName);
            bool success = match.Success;
            return success;
        }
    }
}