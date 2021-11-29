namespace MassTransit
{
    public class RabbitMqSslOptions
    {
        public string ServerName { get; set; }
        public bool Trust { get; set; }
        public string CertPath { get; set; }
        public string CertPassphrase { get; set; }
        public bool CertIdentity { get; set; }
    }
}
