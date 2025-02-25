namespace MassTransit.SqlTransport
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.Middleware;
    using Topology;


    public abstract class SqlClientContext :
        ScopePipeContext,
        ClientContext
    {
        protected SqlClientContext(ConnectionContext context, CancellationToken cancellationToken)
            : base(context)
        {
            ConnectionContext = context;
            CancellationToken = cancellationToken;
        }

        public override CancellationToken CancellationToken { get; }

        public ConnectionContext ConnectionContext { get; }

        public abstract Task<long> CreateQueue(Queue queue);
        public abstract Task<long> CreateTopic(Topic topic);
        public abstract Task<long> CreateTopicSubscription(TopicToTopicSubscription subscription);
        public abstract Task<long> CreateQueueSubscription(TopicToQueueSubscription subscription);
        public abstract Task<long> PurgeQueue(string queueName, CancellationToken cancellationToken);

        public abstract Task Send<T>(string queueName, SqlMessageSendContext<T> context)
            where T : class;

        public abstract Task Publish<T>(string topicName, SqlMessageSendContext<T> context)
            where T : class;

        public abstract Task<IEnumerable<SqlTransportMessage>> ReceiveMessages(string queueName, SqlReceiveMode mode, int messageLimit, int concurrentLimit,
            TimeSpan lockDuration);

        public abstract Task TouchQueue(string queueName);
        public abstract Task<int?> DeadLetterQueue(string queueName, int messageCount);

        public abstract Task<bool> DeleteMessage(Guid lockId, long messageDeliveryId);
        public abstract Task<bool> DeleteScheduledMessage(Guid tokenId, CancellationToken cancellationToken);
        public abstract Task<bool> MoveMessage(Guid lockId, long messageDeliveryId, string queueName, SqlQueueType queueType, SendHeaders sendHeaders);
        public abstract Task<bool> RenewLock(Guid lockId, long messageDeliveryId, TimeSpan duration);
        public abstract Task<bool> Unlock(Guid lockId, long messageDeliveryId, TimeSpan delay, SendHeaders sendHeaders);
    }
}
