namespace MassTransit.KafkaIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Logging;
    using MassTransit.Middleware;
    using Transports;
    using Util;


    public class KafkaMessageConsumer<TKey, TValue> :
        ConsumerAgent<TopicPartitionOffset>,
        KafkaConsumerBuilderContext,
        IKafkaMessageConsumer<TKey, TValue>
        where TValue : class
    {
        readonly CancellationTokenSource _cancellationTokenSource;
        readonly CancellationTokenSource _checkpointTokenSource;
        readonly IConsumer<byte[], byte[]> _consumer;
        readonly KafkaReceiveEndpointContext<TKey, TValue> _context;
        readonly IChannelExecutorPool<ConsumeResult<byte[], byte[]>> _executorPool;
        readonly ConsumerLockContext _lockContext;
        readonly ReceiveSettings _receiveSettings;

        public KafkaMessageConsumer(ReceiveSettings receiveSettings, KafkaReceiveEndpointContext<TKey, TValue> context, ConsumerContext consumerContext)
            : base(context)
        {
            _receiveSettings = receiveSettings;
            _context = context;

            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(Stopping);
            _checkpointTokenSource = CancellationTokenSource.CreateLinkedTokenSource(Stopped);
            _lockContext = new ConsumerLockContext(consumerContext, receiveSettings, _checkpointTokenSource.Token);

            _consumer = consumerContext.CreateConsumer(this, HandleKafkaError);

            _executorPool = new CombinedChannelExecutorPool(_lockContext, receiveSettings);

            TrySetConsumeTask(Task.Run(() => Consume()));
        }

        public IEnumerable<TopicPartitionOffset> OnAssigned(IConsumer<byte[], byte[]> consumer, IEnumerable<TopicPartition> partitions)
        {
            SetReady();
            return _lockContext.OnAssigned(consumer, partitions);
        }

        public IEnumerable<TopicPartitionOffset> OnUnAssigned(IConsumer<byte[], byte[]> consumer, IEnumerable<TopicPartitionOffset> partitions)
        {
            return _lockContext.OnUnAssigned(consumer, partitions);
        }

        public IEnumerable<TopicPartitionOffset> OnPartitionLost(IConsumer<byte[], byte[]> consumer, IEnumerable<TopicPartitionOffset> partitions)
        {
            return _lockContext.OnPartitionLost(consumer, partitions);
        }

        async Task Consume()
        {
            _consumer.Subscribe(_receiveSettings.Topic);

            try
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    ConsumeResult<byte[], byte[]> consumeResult = _consumer.Consume(_cancellationTokenSource.Token);
                    await _lockContext.Pending(consumeResult).ConfigureAwait(false);
                    await _executorPool.Push(consumeResult, () => Handle(consumeResult), Stopping).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException exception) when (exception.CancellationToken == Stopping
                                                               || exception.CancellationToken == _cancellationTokenSource.Token)
            {
            }
            catch (Exception exception)
            {
                LogContext.Warning?.Log(exception, "Consume Loop faulted");
            }
        }

        async Task Handle(ConsumeResult<byte[], byte[]> result)
        {
            if (IsStopping)
                return;

            var context = new KafkaReceiveContext<TKey, TValue>(result, _context);
            var cancellationToken = context.CancellationToken;

            CancellationTokenRegistration? registration = null;
            if (cancellationToken.CanBeCanceled)
                registration = cancellationToken.Register(() => _lockContext.Canceled(result, cancellationToken));

            try
            {
                await Dispatch(result.TopicPartitionOffset, context, new KafkaReceiveLockContext(result, _lockContext)).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                context.LogTransportFaulted(exception);
            }
            finally
            {
                registration?.Dispose();
                context.Dispose();
            }
        }

        void HandleKafkaError(IConsumer<byte[], byte[]> consumer, Error error)
        {
            if (_cancellationTokenSource.IsCancellationRequested)
                return;

            EnabledLogger? logger = error.IsFatal ? LogContext.Error : LogContext.Warning;
            logger?.Log("Consumer [{MemberId}] error ({Code}): {Reason} on {Topic}. IsFatal({IsFatal})", consumer.MemberId, error.Code, error.Reason,
                _receiveSettings.Topic, error.IsFatal);

            if (error.IsLocalError || error.IsFatal)
            {
                _cancellationTokenSource.Cancel();
                SetNotReady(new KafkaException(error));
            }
        }

        protected override async Task ActiveAndActualAgentsCompleted(StopContext context)
        {
            await base.ActiveAndActualAgentsCompleted(context).ConfigureAwait(false);

            await _executorPool.DisposeAsync().ConfigureAwait(false);

            // It is time to cancel pending tasks as we already drained current pool
            _checkpointTokenSource.Cancel();

            try
            {
                _consumer.Close();
            }
            catch (Exception e)
            {
                LogContext.Error?.Log(e, "Consumer [{MemberId}] close faulted on {Topic}", _consumer.MemberId, _receiveSettings.Topic);
            }

            try
            {
                _consumer.Dispose();
            }
            catch (Exception)
            {
                //ignored
            }

            _cancellationTokenSource.Dispose();

            await _lockContext.DisposeAsync().ConfigureAwait(false);
            _checkpointTokenSource.Dispose();
        }


        class CombinedChannelExecutorPool :
            IChannelExecutorPool<ConsumeResult<byte[], byte[]>>
        {
            readonly IChannelExecutorPool<ConsumeResult<byte[], byte[]>> _keyExecutorPool;
            readonly IChannelExecutorPool<ConsumeResult<byte[], byte[]>> _partitionExecutorPool;

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

            public ValueTask DisposeAsync()
            {
                return _keyExecutorPool.DisposeAsync();
            }
        }
    }
}
