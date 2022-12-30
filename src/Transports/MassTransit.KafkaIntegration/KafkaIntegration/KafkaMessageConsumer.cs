namespace MassTransit.KafkaIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Internals;
    using Logging;
    using MassTransit.Middleware;
    using Transports;
    using Util;


    public class KafkaMessageConsumer<TKey, TValue> :
        Agent,
        IKafkaMessageConsumer<TKey, TValue>
        where TValue : class
    {
        readonly ReceiveSettings _receiveSettings;
        readonly KafkaReceiveEndpointContext<TKey, TValue> _context;
        readonly TaskCompletionSource<bool> _deliveryComplete;
        readonly IReceivePipeDispatcher _dispatcher;
        readonly CancellationTokenSource _cancellationTokenSource;
        readonly IConsumer<byte[], byte[]> _consumer;
        readonly Task _task;
        readonly IChannelExecutorPool<ConsumeResult<byte[], byte[]>> _executorPool;
        readonly CancellationTokenSource _checkpointTokenSource;
        readonly IConsumerLockContext _lockContext;

        public KafkaMessageConsumer(ReceiveSettings receiveSettings, KafkaReceiveEndpointContext<TKey, TValue> context, ConsumerContext consumerContext)
        {
            _receiveSettings = receiveSettings;
            _context = context;

            _deliveryComplete = TaskUtil.GetTask<bool>();
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(Stopping);
            _checkpointTokenSource = CancellationTokenSource.CreateLinkedTokenSource(Stopped);
            var lockContext = new ConsumerLockContext(consumerContext, receiveSettings, _checkpointTokenSource.Token);

            _dispatcher = context.CreateReceivePipeDispatcher();
            _consumer = consumerContext.CreateConsumer(lockContext, HandleKafkaError);
            _dispatcher.ZeroActivity += HandleDeliveryComplete;

            _executorPool = new CombinedChannelExecutorPool(lockContext, receiveSettings);
            _lockContext = lockContext;
            _task = Task.Run(Consume);
        }

        public long DeliveryCount => _dispatcher.DispatchCount;

        public int ConcurrentDeliveryCount => _dispatcher.MaxConcurrentDispatchCount;

        async Task Consume()
        {
            _consumer.Subscribe(_receiveSettings.Topic);

            SetReady();

            try
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    ConsumeResult<byte[], byte[]> consumeResult = _consumer.Consume(_cancellationTokenSource.Token);
                    await _lockContext.Pending(consumeResult).ConfigureAwait(false);
                    await _executorPool.Push(consumeResult, () => Handle(consumeResult), Stopping).ConfigureAwait(false);
                }

                SetCompleted(Task.CompletedTask);
            }
            catch (OperationCanceledException exception) when (exception.CancellationToken == Stopping
                                                               || exception.CancellationToken == _cancellationTokenSource.Token)
            {
                SetCompleted(Task.CompletedTask);
            }
            catch (Exception exception)
            {
                LogContext.Error?.Log(exception, "Consume Loop faulted");

                SetCompleted(TaskUtil.Faulted<bool>(exception));
            }
        }

        async Task Handle(ConsumeResult<byte[], byte[]> result)
        {
            if (IsStopping)
                return;

            var context = new KafkaReceiveContext<TKey, TValue>(result, _context, _lockContext);

            try
            {
                await _dispatcher.Dispatch(context, context).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                context.LogTransportFaulted(exception);
            }
            finally
            {
                context.Dispose();
            }
        }

        void HandleKafkaError(IConsumer<byte[], byte[]> consumer, Error error)
        {
            EnabledLogger? logger = error.IsFatal ? LogContext.Error : LogContext.Warning;
            logger?.Log("Consumer [{MemberId}] error ({Code}): {Reason} on {Topic}", consumer.MemberId, error.Code, error.Reason, _receiveSettings.Topic);

            if (_cancellationTokenSource.IsCancellationRequested)
                return;

            var activeDispatchCount = _dispatcher.ActiveDispatchCount;
            if (activeDispatchCount == 0 || error.IsLocalError)
            {
                _cancellationTokenSource.Cancel();
                _deliveryComplete.TrySetResult(true);
                SetCompleted(TaskUtil.Faulted<bool>(new KafkaException(error)));
            }
        }

        async Task HandleDeliveryComplete()
        {
            if (IsStopping)
            {
                LogContext.Debug?.Log("Consumer shutdown completed: {InputAddress}", _context.InputAddress);

                _deliveryComplete.TrySetResult(true);
            }
        }

        protected override Task StopAgent(StopContext context)
        {
            LogContext.Debug?.Log("Stopping consumer: {InputAddress}", _context.InputAddress);

            SetCompleted(ActiveAndActualAgentsCompleted(context));

            return Completed;
        }

        async Task ActiveAndActualAgentsCompleted(StopContext context)
        {
            if (_dispatcher.ActiveDispatchCount > 0)
            {
                try
                {
                    await _deliveryComplete.Task.OrCanceled(context.CancellationToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    LogContext.Warning?.Log("Stop canceled waiting for message consumers to complete: {InputAddress}", _context.InputAddress);
                }
            }

            await _task.ConfigureAwait(false);
            await _executorPool.DisposeAsync().ConfigureAwait(false);

            // It is time to cancel pending tasks as we already drained current pool
            _checkpointTokenSource.Cancel();

            _consumer.Close();

            _consumer.Dispose();
            _cancellationTokenSource.Dispose();
            _checkpointTokenSource.Dispose();
        }


        class CombinedChannelExecutorPool :
            IChannelExecutorPool<ConsumeResult<byte[], byte[]>>
        {
            readonly IChannelExecutorPool<ConsumeResult<byte[], byte[]>> _partitionExecutorPool;
            readonly IChannelExecutorPool<ConsumeResult<byte[], byte[]>> _keyExecutorPool;

            public CombinedChannelExecutorPool(IChannelExecutorPool<ConsumeResult<byte[], byte[]>> partitionExecutorPool, ReceiveSettings receiveSettings)
            {
                _partitionExecutorPool = partitionExecutorPool;
                IHashGenerator hashGenerator = new Murmur3UnsafeHashGenerator();
                _keyExecutorPool = new PartitionChannelExecutorPool<ConsumeResult<byte[], byte[]>>(x => x.Message.Key, hashGenerator,
                    receiveSettings.ConcurrentMessageLimit,
                    receiveSettings.ConcurrentDeliveryLimit);
            }

            public Task Push(ConsumeResult<byte[], byte[]> result, Func<Task> handle, CancellationToken cancellationToken)
            {
                return _partitionExecutorPool.Push(result, () => _keyExecutorPool.Run(result, handle, cancellationToken), cancellationToken);
            }

            public Task Run(ConsumeResult<byte[], byte[]> result, Func<Task> method, CancellationToken cancellationToken = default)
            {
                return _partitionExecutorPool.Run(result, () => _keyExecutorPool.Run(result, method, cancellationToken), cancellationToken);
            }

            public async ValueTask DisposeAsync()
            {
                await _partitionExecutorPool.DisposeAsync().ConfigureAwait(false);
                await _keyExecutorPool.DisposeAsync().ConfigureAwait(false);
            }
        }
    }
}
