namespace MassTransit.AmazonSqsTransport
{
    using System;
    using Amazon;
    using Transport;
    using Transports;


    /// <summary>
    /// Settings to configure a AmazonSQS host explicitly without requiring the fluent interface
    /// </summary>
    public interface AmazonSqsHostSettings
    {
        /// <summary>
        ///     The AmazonSQS region to connect
        /// </summary>
        RegionEndpoint Region { get; }

        /// <summary>
        ///     The AccessKey for connecting to the host
        /// </summary>
        string AccessKey { get; }

        /// <summary>
        ///     The password for connection to the host
        ///     MAYBE this should be a SecureString instead of a regular string
        /// </summary>
        string SecretKey { get; }

        AllowTransportHeader AllowTransportHeader { get; }

        Uri HostAddress { get; }

        IConnection CreateConnection();
    }
}
