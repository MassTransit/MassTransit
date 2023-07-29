namespace MassTransit
{
    using System.Security.Authentication;
    using RabbitMqTransport.Configuration;


    public class RabbitMqSslOptions
    {
        public string ServerName { get; set; }
        public bool Trust { get; set; }
        public string CertPath { get; set; }
        public string CertPassphrase { get; set; }
        public bool CertIdentity { get; set; }
        public SslProtocols Protocol { get; set; } = ConfigurationHostSettings.DefaultSslProtocols;
    }
}
