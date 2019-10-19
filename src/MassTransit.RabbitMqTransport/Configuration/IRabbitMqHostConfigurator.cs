namespace MassTransit.RabbitMqTransport
{
    using System;


    public interface IRabbitMqHostConfigurator
    {
        /// <summary>
        /// Enables RabbitMQ publish acknowledgement, so that the Task returned from Send/Publish
        /// is not completed until the message has been confirmed by the broker.
        /// </summary>
        bool PublisherConfirmation { set; }

        /// <summary>
        /// Configure the use of SSL to connection to RabbitMQ
        /// </summary>
        /// <param name="configureSsl"></param>
        void UseSsl(Action<IRabbitMqSslConfigurator> configureSsl);

        /// <summary>
        /// Specifies the heartbeat interval, in seconds, used to maintain the connection to RabbitMQ.
        /// Setting this value to zero will disable heartbeats, allowing the connection to timeout
        /// after an inactivity period.
        /// </summary>
        /// <param name="requestedHeartbeat"></param>
        void Heartbeat(ushort requestedHeartbeat);

        /// <summary>
        /// Sets the username for the connection to RabbitMQ
        /// </summary>
        /// <param name="username"></param>
        void Username(string username);

        /// <summary>
        /// Sets the password for the connection to RabbitMQ
        /// </summary>
        /// <param name="password"></param>
        void Password(string password);

        /// <summary>
        /// Configure a RabbitMQ High-Availability cluster which will cycle hosts when connections are interrupted.
        /// </summary>
        /// <param name="configureCluster">The cluster configuration</param>
        void UseCluster(Action<IRabbitMqClusterConfigurator> configureCluster);

        /// <summary>
        /// Set the maximum number of channels allowed for the connection
        /// </summary>
        /// <param name="value"></param>
        void RequestedChannelMax(ushort value);

        /// <summary>
        /// The requested connection timeout, in milliseconds
        /// </summary>
        /// <param name="milliseconds"></param>
        void RequestedConnectionTimeout(int milliseconds);
    }
}
