namespace MassTransit.ActiveMqTransport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Apache.NMS;
    using Apache.NMS.Util;
    using Internals;
    using MassTransit.Middleware;
    using Topology;
    using Transports;
    using Util;


    public class ActiveMqSessionContext :
        ScopePipeContext,
        SessionContext,
        IAsyncDisposable
    {
        readonly TaskExecutor _executor;
        readonly MessageProducerCache _messageProducerCache;
        readonly ISession _session;

        public ActiveMqSessionContext(ConnectionContext connectionContext, ISession session, CancellationToken cancellationToken)
            : base(connectionContext)
        {
            ConnectionContext = connectionContext;
            _session = session;
            CancellationToken = cancellationToken;

            _executor = new TaskExecutor();

            _messageProducerCache = new MessageProducerCache();
        }

        public async ValueTask DisposeAsync()
        {
            if (_session != null)
            {
                try
                {
                    await _messageProducerCache.Stop(CancellationToken.None).ConfigureAwait(false);

                    await _session.CloseAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    LogContext.Warning?.Log(ex, "Close session faulted: {Host}", ConnectionContext.Description);
                }

                _session.Dispose();
            }

            await _executor.DisposeAsync().ConfigureAwait(false);
        }

        public override CancellationToken CancellationToken { get; }

        public ISession Session => _session;

        public ConnectionContext ConnectionContext { get; }

        public Task<ITopic> GetTopic(Topic topic)
        {
            return _executor.Run(() =>
            {
                if (!topic.Durable && topic.AutoDelete
                    && topic.EntityName.StartsWith(ConnectionContext.Topology.PublishTopology.VirtualTopicPrefix, StringComparison.InvariantCulture))
                    return ConnectionContext.GetTemporaryTopic(_session, topic.EntityName);

                return SessionUtil.GetTopic(_session, topic.EntityName);
            }, CancellationToken);
        }

        public Task<IQueue> GetQueue(Queue queue)
        {
            return _executor.Run(() =>
            {
                if (!queue.Durable && queue.AutoDelete && !ConnectionContext.IsVirtualTopicConsumer(queue.EntityName))
                    return ConnectionContext.GetTemporaryQueue(_session, queue.EntityName);

                return SessionUtil.GetQueue(_session, queue.EntityName);
            }, CancellationToken);
        }

        public Task<IDestination> GetDestination(string destinationName, DestinationType destinationType)
        {
            if ((destinationType == DestinationType.Queue || destinationType == DestinationType.TemporaryQueue)
                && ConnectionContext.TryGetTemporaryEntity(destinationName, out var destination))
                return Task.FromResult(destination);

            return _executor.Run(() => SessionUtil.GetDestination(_session, destinationName, destinationType), CancellationToken);
        }

        public Task<IMessageConsumer> CreateMessageConsumer(IDestination destination, string selector, bool noLocal)
        {
            return _executor.Run(() => _session.CreateConsumerAsync(destination, selector, noLocal), CancellationToken);
        }

        public async Task SendAsync(IDestination destination, IMessage message, CancellationToken cancellationToken)
        {
            var producer = await _messageProducerCache.GetMessageProducer(destination,
                x => _executor.Run(() => _session.CreateProducerAsync(x), cancellationToken)).ConfigureAwait(false);

            await _executor.Run(() => producer.SendAsync(message, message.NMSDeliveryMode, message.NMSPriority, message.NMSTimeToLive)
                .OrCanceled(cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        public IBytesMessage CreateBytesMessage(byte[] content)
        {
            return _session.CreateBytesMessage(content);
        }

        public ITextMessage CreateTextMessage(string content)
        {
            return _session.CreateTextMessage(content);
        }

        public IMessage CreateMessage()
        {
            return _session.CreateMessage();
        }

        public Task DeleteTopic(string topicName)
        {
            TransportLogMessages.DeleteTopic(topicName);

            return _executor.Run(() =>
            {
                if (!ConnectionContext.TryRemoveTemporaryEntity(_session, topicName))
                    SessionUtil.DeleteTopic(_session, topicName);
            }, CancellationToken.None);
        }

        public Task DeleteQueue(string queueName)
        {
            TransportLogMessages.DeleteQueue(queueName);

            return _executor.Run(() =>
                {
                    if (!ConnectionContext.TryRemoveTemporaryEntity(_session, queueName))
                        SessionUtil.DeleteQueue(_session, queueName);
                }
                , CancellationToken.None);
        }

        public IDestination GetTemporaryDestination(string name)
        {
            return ConnectionContext.TryGetTemporaryEntity(name, out var destination) ? destination : null;
        }
    }
}
