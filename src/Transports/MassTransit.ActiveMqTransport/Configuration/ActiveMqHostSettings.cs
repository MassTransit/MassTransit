namespace MassTransit
{
    using System;
    using Apache.NMS;


    /// <summary>
    /// Settings to configure a ActiveMQ host explicitly without requiring the fluent interface
    /// </summary>
    public interface ActiveMqHostSettings
    {
        /// <summary>
        /// The ActiveMQ host to connect to (should be a valid hostname)
        /// </summary>
        string Host { get; }

        /// <summary>
        /// The ActiveMQ port to connect
        /// </summary>
        int Port { get; }

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
        /// Returns the host address
        /// </summary>
        Uri HostAddress { get; }

        bool UseSsl { get; }

        Uri BrokerAddress { get; }

        IConnection CreateConnection();
    }
}
