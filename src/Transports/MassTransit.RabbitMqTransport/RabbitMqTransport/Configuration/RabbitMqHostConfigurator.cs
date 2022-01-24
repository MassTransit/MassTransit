namespace MassTransit.RabbitMqTransport.Configuration
{
    using System;
    using System.Security.Authentication;


    public class RabbitMqHostConfigurator :
        IRabbitMqHostConfigurator
    {
        static readonly char[] _pathSeparator = { '/' };
        readonly ConfigurationHostSettings _settings;

        public RabbitMqHostConfigurator(Uri hostAddress, string connectionName = null)
        {
            _settings = hostAddress.GetConfigurationHostSettings();

            if (_settings.Port == 5671)
            {
                UseSsl(s =>
                {
                    s.Protocol = SslProtocols.Tls12;
                });
            }

            _settings.VirtualHost = Uri.UnescapeDataString(GetVirtualHost(hostAddress));

            if (!string.IsNullOrEmpty(connectionName))
                _settings.ClientProvidedName = connectionName;
        }

        public RabbitMqHostConfigurator(string host, string virtualHost, ushort port = 5672, string connectionName = null)
        {
            _settings = new ConfigurationHostSettings
            {
                Host = host,
                Port = port,
                VirtualHost = virtualHost
            };

            if (!string.IsNullOrEmpty(connectionName))
                _settings.ClientProvidedName = connectionName;
        }

        public RabbitMqHostSettings Settings => _settings;

        public bool PublisherConfirmation
        {
            set => _settings.PublisherConfirmation = value;
        }

        public void UseSsl(Action<IRabbitMqSslConfigurator> configureSsl)
        {
            var configurator = new RabbitMqSslConfigurator(_settings);

            configureSsl(configurator);

            _settings.Ssl = true;
            _settings.ClientCertificatePassphrase = configurator.CertificatePassphrase;
            _settings.ClientCertificatePath = configurator.CertificatePath;
            _settings.ClientCertificate = configurator.Certificate;
            _settings.UseClientCertificateAsAuthenticationIdentity = configurator.UseCertificateAsAuthenticationIdentity;
            _settings.AcceptablePolicyErrors = configurator.AcceptablePolicyErrors;
            _settings.SslServerName = configurator.ServerName ?? _settings.Host;
            _settings.SslProtocol = configurator.Protocol;
            _settings.CertificateSelectionCallback = configurator.CertificateSelectionCallback;
            _settings.CertificateValidationCallback = configurator.CertificateValidationCallback;
        }

        public void ConfigureBatchPublish(Action<IRabbitMqBatchPublishConfigurator> configure)
        {
            _settings.ConfigureBatch(settings =>
            {
                var configurator = new RabbitMqBatchPublishConfigurator(settings);

                configure?.Invoke(configurator);
            });
        }

        public void ContinuationTimeout(TimeSpan timeout)
        {
            _settings.ContinuationTimeout = timeout;
        }

        public RefreshConnectionFactoryCallback OnRefreshConnectionFactory
        {
            set => _settings.OnRefreshConnectionFactory = value;
        }

        public void Heartbeat(ushort requestedHeartbeat)
        {
            _settings.Heartbeat = TimeSpan.FromSeconds(requestedHeartbeat);
        }

        public void Heartbeat(TimeSpan timeSpan)
        {
            _settings.Heartbeat = timeSpan;
        }

        public void Username(string username)
        {
            _settings.Username = username;
        }

        public void Password(string password)
        {
            _settings.Password = password;
        }

        public void UseCluster(Action<IRabbitMqClusterConfigurator> configureCluster)
        {
            var configurator = new RabbitMqClusterConfigurator(_settings);
            configureCluster(configurator);

            _settings.EndpointResolver = configurator.GetEndpointResolver();
        }

        public void RequestedChannelMax(ushort value)
        {
            _settings.RequestedChannelMax = value;
        }

        public void RequestedConnectionTimeout(int milliseconds)
        {
            _settings.RequestedConnectionTimeout = TimeSpan.FromMilliseconds(milliseconds);
        }

        public void RequestedConnectionTimeout(TimeSpan timeSpan)
        {
            _settings.RequestedConnectionTimeout = timeSpan;
        }

        string GetVirtualHost(Uri address)
        {
            var segments = address.AbsolutePath.Split(_pathSeparator, StringSplitOptions.RemoveEmptyEntries);

            if (segments.Length == 0)
                return "/";

            if (segments.Length == 1)
                return segments[0];

            throw new FormatException("The host path must be empty or contain a single virtual host name");
        }
    }
}
