namespace MassTransit.RabbitMqTransport.Configurators
{
    using System;
    using System.Net.Security;
    using System.Security.Authentication;
    using System.Security.Cryptography.X509Certificates;
    using Metadata;
    using RabbitMQ.Client;


    class ConfigurationHostSettings :
        RabbitMqHostSettings
    {
        readonly Lazy<Uri> _hostAddress;

        public ConfigurationHostSettings()
        {
            var defaultOptions = new SslOption();
            SslProtocol = defaultOptions.Version;
            AcceptablePolicyErrors = defaultOptions.AcceptablePolicyErrors | SslPolicyErrors.RemoteCertificateChainErrors;

            PublisherConfirmation = true;

            RequestedConnectionTimeout = 10000;

            ClientProvidedName = HostMetadataCache.Host.ProcessName;

            _hostAddress = new Lazy<Uri>(FormatHostAddress);
        }

        public string Host { get; set; }
        public int Port { get; set; }
        public string VirtualHost { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public ushort Heartbeat { get; set; }
        public bool Ssl { get; set; }
        public SslProtocols SslProtocol { get; set; }
        public string SslServerName { get; set; }
        public SslPolicyErrors AcceptablePolicyErrors { get; set; }
        public string ClientCertificatePath { get; set; }
        public string ClientCertificatePassphrase { get; set; }
        public X509Certificate ClientCertificate { get; set; }
        public bool UseClientCertificateAsAuthenticationIdentity { get; set; }
        public LocalCertificateSelectionCallback CertificateSelectionCallback { get; set; }
        public RemoteCertificateValidationCallback CertificateValidationCallback { get; set; }
        public string[] ClusterMembers { get; set; }
        public IRabbitMqEndpointResolver HostNameSelector { get; set; }
        public string ClientProvidedName { get; set; }
        public bool PublisherConfirmation { get; set; }
        public Uri HostAddress => _hostAddress.Value;
        public ushort RequestedChannelMax { get; set; }
        public int RequestedConnectionTimeout { get; set; }

        Uri FormatHostAddress()
        {
            return new RabbitMqHostAddress(Host, Port, VirtualHost);
        }
    }
}
