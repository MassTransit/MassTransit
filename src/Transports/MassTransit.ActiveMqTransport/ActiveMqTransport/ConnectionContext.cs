namespace MassTransit.ActiveMqTransport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Apache.NMS;


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
        /// Create a model on the connection
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ISession> CreateSession(CancellationToken cancellationToken);

        bool IsVirtualTopicConsumer(string name);

        IQueue GetTemporaryQueue(ISession session, string topicName);

        ITopic GetTemporaryTopic(ISession session, string topicName);

        bool TryGetTemporaryEntity(string name, out IDestination destination);

        bool TryRemoveTemporaryEntity(ISession session, string name);
    }
}
