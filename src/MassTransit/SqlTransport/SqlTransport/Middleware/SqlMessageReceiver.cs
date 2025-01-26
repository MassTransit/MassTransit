namespace MassTransit.SqlTransport.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.Middleware;
    using Transports;
    using Util;


    /// <summary>
    /// Receives messages from AmazonSQS, pushing them to the InboundPipe of the service endpoint.
    /// </summary>
    public sealed class SqlMessageReceiver :
        ConsumerAgent<Guid>
    {
        readonly ClientContext _client;
        readonly SqlReceiveEndpointContext _context;
        readonly OrderedChannelExecutorPool _executorPool;
        readonly object _lock = new();
        readonly ReceiveSettings _receiveSettings;
        readonly TimeSpan? _touchQueueInterval;
        CancellationTokenSource _cancellationTokenSource;
        DateTime? _lastMaintenance;
        DateTime? _lastTouched;

        /// <summary>
        /// The basic consumer receives messages pushed from the broker.
        /// </summary>
        /// <param name="client">The model context for the consumer</param>
        /// <param name="context">The topology</param>
        public SqlMessageReceiver(ClientContext client, SqlReceiveEndpointContext context)
            : base(context)
        {
            _client = client;
            _context = context;

            _receiveSettings = client.GetPayload<ReceiveSettings>();

            if (_receiveSettings.AutoDeleteOnIdle.HasValue)
                _touchQueueInterval = new TimeSpan(_receiveSettings.AutoDeleteOnIdle.Value.Ticks / 2);

            _executorPool = new OrderedChannelExecutorPool(_receiveSettings);

            TrySetConsumeTask(Task.Run(() => Consume()));
        }

        protected override async Task ActiveAndActualAgentsCompleted(StopContext context)
        {
            await base.ActiveAndActualAgentsCompleted(context).ConfigureAwait(false);

            await _executorPool.DisposeAsync().ConfigureAwait(false);
        }

        async Task Consume()
        {
            using var algorithm = new RequestRateAlgorithm(new RequestRateAlgorithmOptions
            {
                PrefetchCount = _receiveSettings.PrefetchCount,
                ConcurrentResultLimit = _context.ConcurrentMessageLimit ?? _context.PrefetchCount,
                RequestResultLimit = _receiveSettings.PrefetchCount
            });

            SetReady();

            Task Handle(SqlTransportMessage message, CancellationToken cancellationToken)
            {
                var lockContext = new SqlReceiveLockContext(_context.InputAddress, message, _receiveSettings, _client);

                return _receiveSettings.ReceiveMode == SqlReceiveMode.Normal
                    ? HandleMessage(message, lockContext)
                    : _executorPool.Run(message, () => HandleMessage(message, lockContext), cancellationToken);
            }

            try
            {
                while (!IsStopping)
                    await algorithm.Run((messageLimit, token) => ReceiveMessages(messageLimit, token), (m, c) => Handle(m, c), Stopping).ConfigureAwait(false);
            }
            catch (OperationCanceledException exception) when (exception.CancellationToken == Stopping)
            {
            }
            catch (Exception exception)
            {
                LogContext.Warning?.Log(exception, "Consume Loop faulted");
            }
        }

        async Task HandleMessage(SqlTransportMessage message, SqlReceiveLockContext lockContext)
        {
            if (IsStopping)
                return;

            if (message.ExpirationTime.HasValue && message.ExpirationTime.Value < DateTime.UtcNow)
            {
                if (_receiveSettings.DeadLetterExpiredMessages)
                    await lockContext.Expired().ConfigureAwait(false);
                else
                    await lockContext.Complete().ConfigureAwait(false);
            }
            else
            {
                var context = new SqlReceiveContext(message, _context, _receiveSettings, _client, _client.ConnectionContext, lockContext);
                try
                {
                    await Dispatch(message.TransportMessageId, context, lockContext).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    context.LogTransportFaulted(exception);
                }
                finally
                {
                    context.Dispose();
                }

                MessageHandled();
            }
        }

        async Task<IEnumerable<SqlTransportMessage>> ReceiveMessages(int messageLimit, CancellationToken cancellationToken)
        {
            try
            {
                IList<SqlTransportMessage> messages = (await _client.ReceiveMessages(_receiveSettings.EntityName, _receiveSettings.ReceiveMode, messageLimit,
                    _receiveSettings.ConcurrentDeliveryLimit, _receiveSettings.LockDuration).ConfigureAwait(false)).ToList();

                if (messages.Count > 0)
                    return messages;

                try
                {
                    int? count = 0;

                    if (_lastMaintenance.HasValue == false || _lastMaintenance.Value + TimeSpan.FromSeconds(30) > DateTime.UtcNow)
                    {
                        count = await _client.DeadLetterQueue(_receiveSettings.QueueName, _receiveSettings.MaintenanceBatchSize).ConfigureAwait(false);

                        if (count < _receiveSettings.MaintenanceBatchSize)
                            _lastMaintenance = DateTime.UtcNow;
                    }

                    if (_touchQueueInterval.HasValue && count is null or 0)
                    {
                        if (_lastTouched.HasValue == false || _lastTouched.Value + _touchQueueInterval.Value > DateTime.UtcNow)
                        {
                            await _client.TouchQueue(_receiveSettings.EntityName).ConfigureAwait(false);

                            _lastTouched = DateTime.UtcNow;
                        }
                    }
                }
                catch (ObjectDisposedException)
                {
                }
                catch (OperationCanceledException)
                {
                }
                catch (TimeoutException)
                {
                }


                await WaitForPollingIntervalOrMessageHandled(cancellationToken).ConfigureAwait(false);

                return messages;
            }
            catch (OperationCanceledException)
            {
                return [];
            }
        }

        async Task WaitForPollingIntervalOrMessageHandled(CancellationToken cancellationToken)
        {
            lock (_lock)
                _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            try
            {
                var delayTask = _receiveSettings.QueueId.HasValue
                    ? _client.ConnectionContext.DelayUntilMessageReady(_receiveSettings.QueueId.Value, _receiveSettings.PollingInterval,
                        _cancellationTokenSource.Token)
                    : Task.Delay(_receiveSettings.PollingInterval, _cancellationTokenSource.Token);

                await delayTask.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                lock (_lock)
                {
                    _cancellationTokenSource.Dispose();
                    _cancellationTokenSource = null;
                }
            }
        }

        public void MessageHandled()
        {
            lock (_lock)
                _cancellationTokenSource?.Cancel();
        }


        class OrderedChannelExecutorPool :
            IChannelExecutorPool<SqlTransportMessage>
        {
            readonly IChannelExecutorPool<SqlTransportMessage> _keyExecutorPool;

            public OrderedChannelExecutorPool(ReceiveSettings receiveSettings)
            {
                IHashGenerator hashGenerator = new Murmur3UnsafeHashGenerator();
                _keyExecutorPool = new PartitionChannelExecutorPool<SqlTransportMessage>(PartitionKeyProvider, hashGenerator,
                    receiveSettings.ConcurrentMessageLimit, receiveSettings.ConcurrentDeliveryLimit);
            }

            public Task Push(SqlTransportMessage result, Func<Task> handle, CancellationToken cancellationToken)
            {
                return _keyExecutorPool.Push(result, handle, cancellationToken);
            }

            public Task Run(SqlTransportMessage result, Func<Task> method, CancellationToken cancellationToken = default)
            {
                return _keyExecutorPool.Run(result, method, cancellationToken);
            }

            public ValueTask DisposeAsync()
            {
                return _keyExecutorPool.DisposeAsync();
            }

            static byte[] PartitionKeyProvider(SqlTransportMessage message)
            {
                return string.IsNullOrEmpty(message.PartitionKey)
                    ? []
                    : Encoding.UTF8.GetBytes(message.PartitionKey);
            }
        }
    }
}
