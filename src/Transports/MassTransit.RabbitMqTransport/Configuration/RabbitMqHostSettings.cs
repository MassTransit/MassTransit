namespace MassTransit
{
    using System;
    using System.Net.Security;
    using System.Security.Authentication;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using RabbitMQ.Client;
    using RabbitMqTransport;
    using RabbitMqTransport.Configuration;


    /// <summary>
    /// Settings to configure a RabbitMQ host explicitly without requiring the fluent interface
    /// </summary>
    public interface RabbitMqHostSettings
    {
        /// <summary>
        /// The RabbitMQ host to connect to (should be a valid hostname)
        /// </summary>
        string Host { get; }

        /// <summary>
        /// The RabbitMQ port to connect
        /// </summary>
        int Port { get; }

        /// <summary>
        /// The virtual host for the connection
        /// </summary>
        string VirtualHost { get; }

        /// <summary>
        /// The Username for connecting to the host
        /// </summary>
        string Username { get; }

        /// <summary>
        /// The password for connection to the host
        /// MAYBE this should be a SecureString instead of a regular string
        /// </summary>
        string Password { get; }

        /// <summary>
        /// The heartbeat interval (in seconds) to keep the host connection alive
        /// </summary>
        TimeSpan Heartbeat { get; }

        /// <summary>
        /// True if SSL is required
        /// </summary>
        bool Ssl { get; }

        /// <summary>
        /// SSL protocol, Tls11 or Tls12 are recommended
        /// </summary>
        SslProtocols SslProtocol { get; }

        /// <summary>
        /// The server name specified on the certificate for the RabbitMQ server
        /// </summary>
        string SslServerName { get; }

        /// <summary>
        /// The acceptable policy errors for the SSL connection
        /// </summary>
        SslPolicyErrors AcceptablePolicyErrors { get; }

        /// <summary>
        /// The path to the client certificate if client certificate authentication is used
        /// </summary>
        string ClientCertificatePath { get; }

        /// <summary>
        /// The passphrase for the client certificate found using the <see cref="ClientCertificatePath" />, not required if <see cref="ClientCertificate" /> is populated
        /// </summary>
        string ClientCertificatePassphrase { get; }

        /// <summary>
        /// A certificate to use for client certificate authentication, if not set then the <see cref="ClientCertificatePath" /> and
        /// <see cref="ClientCertificatePassphrase" /> will be used
        /// </summary>
        X509Certificate ClientCertificate { get; }

        /// <summary>
        /// Whether the client certificate should be used for logging in to RabbitMQ, ignoring any username and password set
        /// </summary>
        /// <remarks>
        /// RabbitMQ must be configured correctly for this to work, including enabling the rabbitmq_auth_mechanism_ssl plugin
        /// </remarks>
        bool UseClientCertificateAsAuthenticationIdentity { get; }

        /// <summary>
        /// An optional client specified SSL certificate selection callback.  If this is not specified,
        /// the first valid certificate found will be used.
        /// </summary>
        LocalCertificateSelectionCallback CertificateSelectionCallback { get; set; }

        /// <summary>
        /// An optional client specified SSL certificate validation callback.  If this is not specified,
        /// the default callback will be used in conjunction with the <see cref="P:RabbitMQ.Client.SslOption.AcceptablePolicyErrors" /> property to
        /// determine if the remote server certificate is valid.
        /// </summary>
        RemoteCertificateValidationCallback CertificateValidationCallback { get; set; }

        /// <summary>
        /// The host name selector if used to choose which server to connect
        /// </summary>
        IRabbitMqEndpointResolver EndpointResolver { get; }

        /// <summary>
        /// The client-provided name for the connection (displayed in RabbitMQ admin panel)
        /// </summary>
        string ClientProvidedName { get; }

        /// <summary>
        /// Returns the host address
        /// </summary>
        Uri HostAddress { get; }

        /// <summary>
        /// True if the publisher should confirm acceptance of messages
        /// </summary>
        bool PublisherConfirmation { get; }

        /// <summary>
        /// The maximum number of channels for the connection
        /// </summary>
        ushort RequestedChannelMax { get; }

        /// <summary>
        /// The requested connection timeout, in milliseconds
        /// </summary>
        TimeSpan RequestedConnectionTimeout { get; }

        /// <summary>
        /// Batch settings used for the batch publish
        /// </summary>
        BatchSettings BatchSettings { get; }

        /// <summary>
        /// The confirmation timeout for RPC commands via Models
        /// </summary>
        TimeSpan ContinuationTimeout { get; }

        /// <summary>
        /// Called prior to the connection factory being used to connect, so that any settings can be updated.
        /// Typically this would be the username/password in response to an expired token, etc.
        /// </summary>
        /// <param name="connectionFactory"></param>
        /// <returns></returns>
        Task Refresh(ConnectionFactory connectionFactory);
    }
}
