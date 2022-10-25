namespace MassTransit.ActiveMqTransport
{
    using Apache.NMS;
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;


    public interface ConnectionContext :
        PipeContext
    {
        /// <summary>
        /// The ActiveMQ Connection
        /// </summary>
        IConnection Connection { get; }

        /// <summary>
        /// The connection description, useful to debug output
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The Host Address for this connection
        /// </summary>
        Uri HostAddress { get; }

        IActiveMqBusTopology Topology { get; }

        /// <summary>
        /// Temporary destination are accessible per connection. This map between an queue/topic name and generated temporary name.
        /// </summary>
        ConcurrentDictionary<string, IDestination> TemporaryDestinationMap { get; }

        /// <summary>
        /// Create a model on the connection
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ISession> CreateSession(CancellationToken cancellationToken);
    }
}
