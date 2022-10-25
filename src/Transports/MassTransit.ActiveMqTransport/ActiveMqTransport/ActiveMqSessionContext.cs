namespace MassTransit.ActiveMqTransport
{
    using Apache.NMS;
    using Apache.NMS.Util;
    using MassTransit.ActiveMqTransport.Topology;
    using MassTransit.Middleware;
    using System;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Transports;
    using Util;

    public class ActiveMqSessionContext :
        ScopePipeContext,
        SessionContext,
        IAsyncDisposable
    {
        readonly ChannelExecutor _executor;
        readonly MessageProducerCache _messageProducerCache;
        readonly ISession _session;


        /// <summary>
        /// Regular expression to distinguish if a destination is not for consuming data from a VirtualTopic. If yes we must get a standard destination because the name of the destination must match specific
        /// pattern. A temporary destination has generated name. 
        /// </summary>
        /// <seealso cref="https://activemq.apache.org/virtual-destinations"/>
        readonly Regex _virtualTopicConsumerPattern;

        public ActiveMqSessionContext(ConnectionContext connectionContext, ISession session, CancellationToken cancellationToken)
            : base(connectionContext)
        {
            ConnectionContext = connectionContext;
            _session = session;
            CancellationToken = cancellationToken;

            _executor = new ChannelExecutor(1);

            _messageProducerCache = new MessageProducerCache();

            _virtualTopicConsumerPattern = new Regex(ConnectionContext.Topology.PublishTopology.VirtualTopicConsumerPattern, RegexOptions.Compiled);
        }

        public async ValueTask DisposeAsync()
        {
            if (_session != null)
            {
                try
                {
                    await _messageProducerCache.Stop(CancellationToken.None).ConfigureAwait(false);

                    _session.Close();
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
                {
                    return (ITopic)ConnectionContext.TemporaryDestinationMap.GetOrAdd(topic.EntityName,
                        name => (ITopic)SessionUtil.GetDestination(_session, topic.EntityName, DestinationType.TemporaryTopic));
                }
                return SessionUtil.GetTopic(_session, topic.EntityName);
            }, CancellationToken);
        }

        public Task<IQueue> GetQueue(Queue queue)
        {
            return _executor.Run(() =>
            {
                if (!queue.Durable && queue.AutoDelete && !_virtualTopicConsumerPattern.IsMatch(queue.EntityName))
                {
                    return (IQueue)ConnectionContext.TemporaryDestinationMap.GetOrAdd(queue.EntityName,
                        name => (IQueue)SessionUtil.GetDestination(_session, queue.EntityName, DestinationType.TemporaryQueue));
                }
                return SessionUtil.GetQueue(_session, queue.EntityName);
            }, CancellationToken);
        }

        public Task<IDestination> GetDestination(string destinationName, DestinationType destinationType)
        {
            if ((destinationType == DestinationType.Queue || destinationType == DestinationType.TemporaryQueue)
                && ConnectionContext.TemporaryDestinationMap.TryGetValue(destinationName, out var destination))
            {
                return Task.FromResult(destination);
            }
            return _executor.Run(() => SessionUtil.GetDestination(_session, destinationName, destinationType), CancellationToken);
        }

        public Task<IMessageProducer> CreateMessageProducer(IDestination destination)
        {
            return _messageProducerCache.GetMessageProducer(destination, x => _executor.Run(() => _session.CreateProducer(x), CancellationToken));
        }

        public Task<IMessageConsumer> CreateMessageConsumer(IDestination destination, string selector, bool noLocal)
        {
            return _executor.Run(() => _session.CreateConsumer(destination, selector, noLocal), CancellationToken);
        }

        public IBytesMessage CreateBytesMessage()
        {
            return _session.CreateBytesMessage();
        }

        public Task DeleteTopic(string topicName)
        {
            TransportLogMessages.DeleteTopic(topicName);

            return _executor.Run(() =>
            {
                if (ConnectionContext.TemporaryDestinationMap.TryGetValue(topicName, out var queue))
                {
                    SessionUtil.DeleteDestination(_session, ((ITopic)queue).TopicName, DestinationType.TemporaryTopic);
                    return;
                }
                SessionUtil.DeleteTopic(_session, topicName);
            }, CancellationToken.None);
        }

        public Task DeleteQueue(string queueName)
        {
            TransportLogMessages.DeleteQueue(queueName);

            return _executor.Run(() =>
            {
                if (ConnectionContext.TemporaryDestinationMap.TryGetValue(queueName, out var queue))
                {
                    SessionUtil.DeleteDestination(_session, ((IQueue)queue).QueueName, DestinationType.TemporaryQueue);
                    return;
                }
                SessionUtil.DeleteQueue(_session, queueName);
            }
            , CancellationToken.None);
        }

        public IDestination GetTemporaryDestination(string name)
        {
            if (ConnectionContext.TemporaryDestinationMap.TryGetValue(name, out var destination))
            {
                return destination;
            }
            return null;
        }
    }
}
