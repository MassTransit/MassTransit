namespace MassTransit.KafkaIntegration.Configuration
{
    using Confluent.Kafka;


    public class KafkaSslConfigurator :
        IKafkaSslConfigurator
    {
        readonly ClientConfig _clientConfig;

        public KafkaSslConfigurator(ClientConfig clientConfig)
        {
            _clientConfig = clientConfig;
        }

        public string SslCertificateLocation
        {
            set => _clientConfig.SslCertificateLocation = value;
        }

        public string SslCertificatePem
        {
            set => _clientConfig.SslCertificatePem = value;
        }

        public string SslCaLocation
        {
            set => _clientConfig.SslCaLocation = value;
        }

        public string SslCrlLocation
        {
            set => _clientConfig.SslCrlLocation = value;
        }

        public string SslCaPem
        {
            set => _clientConfig.SslCaPem = value;
        }

        public string SslCaCertificateStores
        {
            set => _clientConfig.SslCaCertificateStores = value;
        }

        public string SslEngineLocation
        {
            set => _clientConfig.SslEngineLocation = value;
        }

        public string SslEngineId
        {
            set => _clientConfig.SslEngineId = value;
        }

        public bool? EnableSslCertificateVerification
        {
            set => _clientConfig.EnableSslCertificateVerification = value;
        }

        public SslEndpointIdentificationAlgorithm? SslEndpointIdentificationAlgorithm
        {
            set => _clientConfig.SslEndpointIdentificationAlgorithm = value;
        }

        public string CipherSuites
        {
            set => _clientConfig.SslCipherSuites = value;
        }

        public string CurvesList
        {
            set => _clientConfig.SslCurvesList = value;
        }

        public string SigalgsList
        {
            set => _clientConfig.SslSigalgsList = value;
        }

        public string KeyLocation
        {
            set => _clientConfig.SslKeyLocation = value;
        }

        public string KeyPassword
        {
            set => _clientConfig.SslKeyPassword = value;
        }

        public string KeyPem
        {
            set => _clientConfig.SslKeyPem = value;
        }

        public string CertificateLocation
        {
            set => _clientConfig.SslCertificateLocation = value;
        }

        public string CertificatePem
        {
            set => _clientConfig.SslCertificatePem = value;
        }

        public string CaLocation
        {
            set => _clientConfig.SslCaLocation = value;
        }

        public string CrlLocation
        {
            set => _clientConfig.SslCrlLocation = value;
        }

        public string KeystoreLocation
        {
            set => _clientConfig.SslKeystoreLocation = value;
        }

        public string KeystorePassword
        {
            set => _clientConfig.SslKeystorePassword = value;
        }

        public bool? EnableCertificateVerification
        {
            set => _clientConfig.EnableSslCertificateVerification = value;
        }

        public SslEndpointIdentificationAlgorithm? EndpointIdentificationAlgorithm
        {
            set => _clientConfig.SslEndpointIdentificationAlgorithm = value;
        }
    }
}
