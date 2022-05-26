namespace MassTransit.AmazonSqsTransport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Topology;


    public interface ConnectionContext :
        PipeContext
    {
        /// <summary>
        /// The Amazon Connection
        /// </summary>
        IConnection Connection { get; }

        /// <summary>
        /// The Host Address for this connection
        /// </summary>
        Uri HostAddress { get; }

        IAmazonSqsBusTopology Topology { get; }

        Task<QueueInfo> GetQueue(Queue queue);
        Task<QueueInfo> GetQueueByName(string name);
        void RemoveQueueByName(string name);

        Task<TopicInfo> GetTopic(Topic topic);
        Task<TopicInfo> GetTopicByName(string name);
        void RemoveTopicByName(string name);

        ClientContext CreateClientContext(CancellationToken cancellationToken);
    }
}
