namespace MassTransit.ActiveMqTransport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Apache.NMS;
    using Apache.NMS.Util;
    using MassTransit.Middleware;
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

        public ActiveMqSessionContext(ConnectionContext connectionContext, ISession session, CancellationToken cancellationToken)
            : base(connectionContext)
        {
            ConnectionContext = connectionContext;
            _session = session;
            CancellationToken = cancellationToken;

            _executor = new ChannelExecutor(1);

            _messageProducerCache = new MessageProducerCache();
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

        public Task<ITopic> GetTopic(string topicName)
        {
            return _executor.Run(() => SessionUtil.GetTopic(_session, topicName), CancellationToken);
        }

        public Task<IQueue> GetQueue(string queueName)
        {
            return _executor.Run(() => SessionUtil.GetQueue(_session, queueName), CancellationToken);
        }

        public Task<IDestination> GetDestination(string destination, DestinationType destinationType)
        {
            return _executor.Run(() => SessionUtil.GetDestination(_session, destination, destinationType), CancellationToken);
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

            return _executor.Run(() => SessionUtil.DeleteTopic(_session, topicName), CancellationToken.None);
        }

        public Task DeleteQueue(string queueName)
        {
            TransportLogMessages.DeleteQueue(queueName);

            return _executor.Run(() => SessionUtil.DeleteQueue(_session, queueName), CancellationToken.None);
        }
    }
}
