namespace MassTransit.RabbitMqTransport.Configuration
{
    using System.Net.Security;
    using System.Security.Authentication;
    using System.Security.Cryptography.X509Certificates;


    public class RabbitMqSslConfigurator :
        IRabbitMqSslConfigurator
    {
        public RabbitMqSslConfigurator(RabbitMqHostSettings settings)
        {
            CertificatePath = settings.ClientCertificatePath;
            CertificatePassphrase = settings.ClientCertificatePassphrase;
            Certificate = settings.ClientCertificate;
            UseCertificateAsAuthenticationIdentity = settings.UseClientCertificateAsAuthenticationIdentity;
            ServerName = settings.SslServerName;
            Protocol = settings.SslProtocol;
            AcceptablePolicyErrors = settings.AcceptablePolicyErrors;
            CertificateSelectionCallback = settings.CertificateSelectionCallback;
            CertificateValidationCallback = settings.CertificateValidationCallback;
        }

        public SslPolicyErrors AcceptablePolicyErrors { get; set; }

        public void AllowPolicyErrors(SslPolicyErrors policyErrors)
        {
            AcceptablePolicyErrors |= policyErrors;
        }

        public void EnforcePolicyErrors(SslPolicyErrors policyErrors)
        {
            AcceptablePolicyErrors &= ~policyErrors;
        }

        public string CertificatePath { get; set; }

        public string CertificatePassphrase { get; set; }

        public X509Certificate Certificate { get; set; }

        public string ServerName { get; set; }

        public SslProtocols Protocol { get; set; }

        public bool UseCertificateAsAuthenticationIdentity { get; set; }

        public LocalCertificateSelectionCallback CertificateSelectionCallback { get; set; }

        public RemoteCertificateValidationCallback CertificateValidationCallback { get; set; }
    }
}
