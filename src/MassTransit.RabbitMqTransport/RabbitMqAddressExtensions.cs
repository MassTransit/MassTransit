namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using Configurators;
    using Metadata;
    using RabbitMQ.Client;
    using Topology;
    using Topology.Settings;
    using Transport;


    public static class RabbitMqAddressExtensions
    {
        public static ReceiveSettings GetReceiveSettings(this Uri address)
        {
            var hostAddress = new RabbitMqHostAddress(address);
            var endpointAddress = new RabbitMqEndpointAddress(hostAddress, address);

            ReceiveSettings settings = new RabbitMqReceiveSettings(endpointAddress.Name, endpointAddress.ExchangeType, endpointAddress.Durable,
                endpointAddress.AutoDelete)
            {
                QueueName = endpointAddress.Name,
                PrefetchCount = hostAddress.Prefetch ?? (ushort)(Environment.ProcessorCount * 2),
                Exclusive = endpointAddress.AutoDelete && !endpointAddress.Durable
            };

            if (hostAddress.TimeToLive.HasValue)
                settings.QueueArguments.Add(Headers.XMessageTTL, hostAddress.TimeToLive.Value.ToString("F0", CultureInfo.InvariantCulture));

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
                RequestedConnectionTimeout = settings.RequestedConnectionTimeout,
                RequestedChannelMax = settings.RequestedChannelMax
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
            var hostAddress = new RabbitMqHostAddress(address);

            var hostSettings = new ConfigurationHostSettings
            {
                Host = hostAddress.Host,
                VirtualHost = hostAddress.VirtualHost,
                Username = "",
                Password = "",
            };

            if (hostAddress.Port.HasValue)
                hostSettings.Port = hostAddress.Port.Value;

            if (!string.IsNullOrEmpty(address.UserInfo))
            {
                string[] parts = address.UserInfo.Split(':');
                hostSettings.Username = parts[0];

                if (parts.Length >= 2)
                    hostSettings.Password = parts[1];
            }

            hostSettings.Heartbeat = hostAddress.Heartbeat ?? (ushort)0;

            return hostSettings;
        }
    }
}
