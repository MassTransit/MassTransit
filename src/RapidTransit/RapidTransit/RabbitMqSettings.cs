namespace RapidTransit
{
    using Configuration;


    public interface RabbitMqSettings :
        ISettings
    {
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
        ushort Heartbeat { get; }

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
        /// A URI-style list of options to use when specifying the input queue
        /// </summary>
        string Options { get; }

        /// <summary>
        /// RabbitMQ uses a server-side policy to match queues that should be HA in a cluster,
        /// so make that a configuration option
        /// </summary>
        string HighAvailabilityQueuePrefix { get; }
    }
}