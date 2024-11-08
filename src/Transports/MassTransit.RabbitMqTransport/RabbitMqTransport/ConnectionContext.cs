namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using RabbitMQ.Client;


    /// <summary>
    /// A RabbitMQ connection
    /// </summary>
    public interface ConnectionContext :
        PipeContext
    {
        /// <summary>
        /// The RabbitMQ Connection
        /// </summary>
        IConnection Connection { get; }

        /// <summary>
        /// The connection description, useful to debug output
        /// </summary>
        string Description { get; }

        Uri HostAddress { get; }

        bool PublisherConfirmation { get; }

        BatchSettings BatchSettings { get; }

        TimeSpan ContinuationTimeout { get; }

        /// <summary>
        /// The time to wait during shutdown of any dependencies before giving up and killing things
        /// </summary>
        TimeSpan StopTimeout { get; }

        IRabbitMqBusTopology Topology { get; }

        /// <summary>
        /// Create a channel on the connection
        /// </summary>
        /// <returns></returns>
        Task<IChannel> CreateChannel(CancellationToken cancellationToken, ushort? concurrentMessageLimit1);

        /// <summary>
        /// Create a channel, and return the <see cref="ChannelContext" />.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="concurrentMessageLimit"></param>
        /// <returns></returns>
        Task<ChannelContext> CreateChannelContext(CancellationToken cancellationToken, ushort? concurrentMessageLimit);
    }
}
