namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using Configuration;
    using Metadata;
    using RabbitMQ.Client;
    using Topology;


    public static class RabbitMqAddressExtensions
    {
        public static ReceiveSettings GetReceiveSettings(this Uri address)
        {
            var hostAddress = new RabbitMqHostAddress(address);
            var endpointAddress = new RabbitMqEndpointAddress(hostAddress, address);

            var topologyConfiguration = new RabbitMqTopologyConfiguration(RabbitMqBusFactory.MessageTopology);
            var endpointConfiguration = new RabbitMqEndpointConfiguration(topologyConfiguration);
            var settings = new RabbitMqReceiveSettings(endpointConfiguration, endpointAddress.Name, endpointAddress.ExchangeType,
                endpointAddress.Durable, endpointAddress.AutoDelete)
            {
                QueueName = endpointAddress.Name,
                Exclusive = endpointAddress.AutoDelete && !endpointAddress.Durable
            };

            if (hostAddress.Prefetch.HasValue)
                settings.PrefetchCount = hostAddress.Prefetch.Value;

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

            if (settings.EndpointResolver != null)
            {
                factory.HostName = null;
                factory.EndpointResolverFactory = x => settings.EndpointResolver;
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

            ApplySslOptions(settings, factory.Ssl);

            factory.ClientProperties ??= new Dictionary<string, object>();

            var hostInfo = HostMetadataCache.Host;

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

            return factory;
        }

        public static void ApplySslOptions(this RabbitMqHostSettings settings, SslOption option)
        {
            option.Enabled = settings.Ssl;
            option.Version = settings.SslProtocol;
            option.AcceptablePolicyErrors = settings.AcceptablePolicyErrors;
            option.ServerName = settings.SslServerName;
            option.Certs = settings.ClientCertificate == null ? null : new X509Certificate2Collection { settings.ClientCertificate };
            option.CertificateSelectionCallback = settings.CertificateSelectionCallback;
            option.CertificateValidationCallback = settings.CertificateValidationCallback;

            if (string.IsNullOrWhiteSpace(option.ServerName))
                option.AcceptablePolicyErrors |= SslPolicyErrors.RemoteCertificateNameMismatch;

            if (string.IsNullOrEmpty(settings.ClientCertificatePath))
            {
                option.CertPath = "";
                option.CertPassphrase = "";
            }
            else
            {
                option.CertPath = settings.ClientCertificatePath;
                option.CertPassphrase = settings.ClientCertificatePassphrase;
            }
        }

        public static RabbitMqHostSettings GetHostSettings(this Uri address)
        {
            return GetConfigurationHostSettings(address);
        }

        public static IRabbitMqBusTopology GetRabbitMqHostTopology(this IBus bus)
        {
            if (bus.Topology is IRabbitMqBusTopology hostTopology)
                return hostTopology;

            throw new ArgumentException("The bus is not a RabbitMQ bus", nameof(bus));
        }

        internal static ConfigurationHostSettings GetConfigurationHostSettings(this Uri address)
        {
            var hostAddress = new RabbitMqHostAddress(address);

            var hostSettings = new ConfigurationHostSettings
            {
                Host = hostAddress.Host,
                VirtualHost = hostAddress.VirtualHost,
                Username = "",
                Password = ""
            };

            if (hostAddress.Port.HasValue)
                hostSettings.Port = hostAddress.Port.Value;

            if (!string.IsNullOrEmpty(address.UserInfo))
            {
                var parts = address.UserInfo.Split(':');
                hostSettings.Username = UriDecode(parts[0]);

                if (parts.Length >= 2)
                    hostSettings.Password = UriDecode(parts[1]);
            }

            hostSettings.Heartbeat = TimeSpan.FromSeconds(hostAddress.Heartbeat ?? 0);

            return hostSettings;
        }

        static string UriDecode(string uri)
        {
            return Uri.UnescapeDataString(uri.Replace("+", "%2B"));
        }
    }
}
