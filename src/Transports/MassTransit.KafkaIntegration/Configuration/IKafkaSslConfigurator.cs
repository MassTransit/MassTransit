namespace MassTransit.KafkaIntegration.Configuration
{
    using Confluent.Kafka;


    public interface IKafkaSslConfigurator
    {
        /// <summary>
        /// A cipher suite is a named combination of authentication, encryption, MAC and key exchange algorithm used to negotiate the security settings for a network connection using TLS
        /// or SSL network protocol. See manual page for `ciphers(1)` and `SSL_CTX_set_cipher_list(3).
        /// default: ''
        /// importance: low
        /// </summary>
        string CipherSuites { set; }

        /// <summary>
        /// The supported-curves extension in the TLS ClientHello message specifies the curves (standard/named, or 'explicit' GF(2^k) or GF(p)) the client is willing to have the server
        /// use. See manual page for `SSL_CTX_set1_curves_list(3)`. OpenSSL &gt;= 1.0.2 required.
        /// default: ''
        /// importance: low
        /// </summary>
        string CurvesList { set; }

        /// <summary>
        /// The client uses the TLS ClientHello signature_algorithms extension to indicate to the server which signature/hash algorithm pairs may be used in digital signatures. See manual
        /// page for `SSL_CTX_set1_sigalgs_list(3)`. OpenSSL &gt;= 1.0.2 required.
        /// default: ''
        /// importance: low
        /// </summary>
        string SigalgsList { set; }

        /// <summary>
        /// Path to client's private key (PEM) used for authentication.
        /// default: ''
        /// importance: low
        /// </summary>
        string KeyLocation { set; }

        /// <summary>
        /// Private key passphrase (for use with `ssl.key.location` and `set_ssl_cert()`)
        /// default: ''
        /// importance: low
        /// </summary>
        string KeyPassword { set; }

        /// <summary>
        /// Client's private key string (PEM format) used for authentication.
        /// default: ''
        /// importance: low
        /// </summary>
        string KeyPem { set; }

        /// <summary>
        /// Path to client's key (PEM) used for authentication.
        /// default: ''
        /// importance: low
        /// </summary>
        string CertificateLocation { set; }

        /// <summary>
        /// Client's key string (PEM format) used for authentication.
        /// default: ''
        /// importance: low
        /// </summary>
        string CertificatePem { set; }

        /// <summary>
        /// File or directory path to CA certificate(s) for verifying the broker's key.
        /// default: ''
        /// importance: low
        /// </summary>
        string CaLocation { set; }

        /// <summary>
        /// Path to CRL for verifying broker's certificate validity.
        /// default: ''
        /// importance: low
        /// </summary>
        string CrlLocation { set; }

        /// <summary>
        /// Path to client's keystore (PKCS#12) used for authentication.
        /// default: ''
        /// importance: low
        /// </summary>
        string KeystoreLocation { set; }

        /// <summary>
        /// Client's keystore (PKCS#12) password.
        /// default: ''
        /// importance: low
        /// </summary>
        string KeystorePassword { set; }

        /// <summary>
        /// Enable OpenSSL's builtin broker (server) certificate verification. This verification can be extended by the application by implementing a certificate_verify_cb.
        /// default: true
        /// importance: low
        /// </summary>
        bool? EnableCertificateVerification { set; }

        /// <summary>
        /// Endpoint identification algorithm to validate broker hostname using broker certificate. https - Server (broker) hostname verification as specified in RFC2818. none - No
        /// endpoint verification. OpenSSL &gt;= 1.0.2 required.
        /// default: none
        /// importance: low
        /// </summary>
        SslEndpointIdentificationAlgorithm? EndpointIdentificationAlgorithm { set; }
    }
}
