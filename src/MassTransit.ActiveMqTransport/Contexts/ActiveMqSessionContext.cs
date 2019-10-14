namespace MassTransit.ActiveMqTransport.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Apache.NMS;
    using Apache.NMS.Util;
    using Context;
    using GreenPipes;
    using Util;


    public class ActiveMqSessionContext :
        ScopePipeContext,
        SessionContext,
        IAsyncDisposable
    {
        readonly ConnectionContext _connectionContext;
        readonly ISession _session;
        readonly CancellationToken _cancellationToken;
        readonly LimitedConcurrencyLevelTaskScheduler _taskScheduler;
        readonly MessageProducerCache _messageProducerCache;

        public ActiveMqSessionContext(ConnectionContext connectionContext, ISession session, CancellationToken cancellationToken)
            : base(connectionContext)
        {
            _connectionContext = connectionContext;
            _session = session;
            _cancellationToken = cancellationToken;

            _taskScheduler = new LimitedConcurrencyLevelTaskScheduler(1);

            _messageProducerCache = new MessageProducerCache();
        }

        public async Task DisposeAsync(CancellationToken cancellationToken)
        {
            LogContext.Debug?.Log("Closing session: {Host}", _connectionContext.Description);

            if (_session != null)
            {
                try
                {
                    await _messageProducerCache.Stop(cancellationToken).ConfigureAwait(false);

                    _session.Close();
                }
                catch (Exception ex)
                {
                    LogContext.Warning?.Log(ex, "Close session faulted: {Host}", _connectionContext.Description);
                }

                _session.Dispose();
            }
        }

        CancellationToken PipeContext.CancellationToken => _cancellationToken;

        ISession SessionContext.Session => _session;

        ConnectionContext SessionContext.ConnectionContext => _connectionContext;

        public Task<ITopic> GetTopic(string topicName)
        {
            return Task.Factory.StartNew(() => SessionUtil.GetTopic(_session, topicName), CancellationToken, TaskCreationOptions.None, _taskScheduler);
        }

        public Task<IQueue> GetQueue(string queueName)
        {
            return Task.Factory.StartNew(() => SessionUtil.GetQueue(_session, queueName), CancellationToken, TaskCreationOptions.None, _taskScheduler);
        }

        public Task<IDestination> GetDestination(string destination, DestinationType destinationType)
        {
            return Task.Factory.StartNew(() => SessionUtil.GetDestination(_session, destination, destinationType), CancellationToken, TaskCreationOptions.None,
                _taskScheduler);
        }

        public Task<IMessageProducer> CreateMessageProducer(IDestination destination)
        {
            return _messageProducerCache.GetMessageProducer(destination, x =>
                Task.Factory.StartNew(() => _session.CreateProducer(x), CancellationToken, TaskCreationOptions.None, _taskScheduler));
        }

        public Task<IMessageConsumer> CreateMessageConsumer(IDestination destination, string selector, bool noLocal)
        {
            return Task.Factory.StartNew(() => _session.CreateConsumer(destination, selector, noLocal), CancellationToken, TaskCreationOptions.None,
                _taskScheduler);
        }

        public Task DeleteTopic(string topicName)
        {
            return Task.Factory.StartNew(() => SessionUtil.DeleteTopic(_session, topicName), CancellationToken.None, TaskCreationOptions.None, _taskScheduler);
        }

        public Task DeleteQueue(string queueName)
        {
            return Task.Factory.StartNew(() => SessionUtil.DeleteQueue(_session, queueName), CancellationToken.None, TaskCreationOptions.None, _taskScheduler);
        }
    }
}
