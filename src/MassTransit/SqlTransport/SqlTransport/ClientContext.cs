namespace MassTransit.SqlTransport
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Topology;


    public interface ClientContext :
        PipeContext
    {
        ConnectionContext ConnectionContext { get; }

        /// <summary>
        /// Create a queue
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        Task<long> CreateQueue(Queue queue);

        /// <summary>
        /// Create a topic
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        Task<long> CreateTopic(Topic topic);

        /// <summary>
        /// Create a topic subscription
        /// </summary>
        /// <param name="subscription"></param>
        /// <returns></returns>
        Task<long> CreateTopicSubscription(TopicToTopicSubscription subscription);

        /// <summary>
        /// Create a topic subscription to a queue
        /// </summary>
        /// <param name="subscription"></param>
        /// <returns></returns>
        Task<long> CreateQueueSubscription(TopicToQueueSubscription subscription);

        /// <summary>
        /// Purge the specified queue (including all queue types), returning the number of messages removed
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<long> PurgeQueue(string queueName, CancellationToken cancellationToken);

        Task Send<T>(string queueName, SqlMessageSendContext<T> context)
            where T : class;

        Task Publish<T>(string topicName, SqlMessageSendContext<T> context)
            where T : class;

        Task<IEnumerable<SqlTransportMessage>> ReceiveMessages(string queueName, SqlReceiveMode mode, int messageLimit, int concurrentCount,
            TimeSpan lockDuration);

        Task TouchQueue(string queueName);

        /// <summary>
        /// Move any messages that have either expired or exceeded their delivery count to the dead-letter queue
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="messageCount"></param>
        /// <returns></returns>
        Task<int?> DeadLetterQueue(string queueName, int messageCount);

        Task<bool> DeleteMessage(Guid lockId, long messageDeliveryId);
        Task<bool> DeleteScheduledMessage(Guid tokenId, CancellationToken cancellationToken);
        Task<bool> MoveMessage(Guid lockId, long messageDeliveryId, string queueName, SqlQueueType queueType, SendHeaders sendHeaders);
        Task<bool> RenewLock(Guid lockId, long messageDeliveryId, TimeSpan duration);
        Task<bool> Unlock(Guid lockId, long messageDeliveryId, TimeSpan delay, SendHeaders sendHeaders);
    }
}
