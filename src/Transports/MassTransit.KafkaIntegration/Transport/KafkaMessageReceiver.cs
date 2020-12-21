namespace MassTransit.KafkaIntegration.Transport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Context;
    using Contexts;
    using GreenPipes.Agents;
    using GreenPipes.Internals.Extensions;
    using Transports;
    using Util;


    public class KafkaMessageReceiver<TKey, TValue> :
        Agent,
        IKafkaMessageReceiver<TKey, TValue>
        where TValue : class
    {
        readonly ReceiveEndpointContext _context;
        readonly IKafkaConsumerContext<TKey, TValue> _consumerContext;
        readonly IReceivePipeDispatcher _dispatcher;
        readonly TaskCompletionSource<bool> _deliveryComplete;
        readonly IConsumerLockContext<TKey, TValue> _consumerLockContext;
        readonly CancellationTokenSource _cancellationTokenSource;
        readonly IConsumer<TKey, TValue> _consumer;

        public KafkaMessageReceiver(ReceiveEndpointContext context, IKafkaConsumerContext<TKey, TValue> consumerContext)
        {
            _context = context;
            _consumerContext = consumerContext;
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(Stopping);

            ConsumerBuilder<TKey, TValue> consumerBuilder = consumerContext.CreateConsumerBuilder()
                .SetLogHandler((c, message) => context.LogContext.Info?.Log(message.Message))
                .SetStatisticsHandler((c, value) => context.LogContext.Debug?.Log(value))
                .SetErrorHandler((c, e) =>
                {
                    if (_cancellationTokenSource.Token.IsCancellationRequested)
                        return;
                    HandleKafkaError(e);
                });

            _consumerLockContext = new ConsumerLockContext<TKey, TValue>(consumerBuilder, context.LogContext,
                _consumerContext.ReceiveSettings.CheckpointInterval, _consumerContext.ReceiveSettings.CheckpointMessageCount);

            _deliveryComplete = TaskUtil.GetTask<bool>();

            _consumer = consumerBuilder.Build();
            _dispatcher = context.CreateReceivePipeDispatcher();
            _dispatcher.ZeroActivity += HandleDeliveryComplete;

            Task.Run(Consume);
        }

        async Task Consume()
        {
            var prefetchCount = Math.Max(1000, _consumerContext.ReceiveSettings.CheckpointMessageCount / 10);
            var executor = new ChannelExecutor(prefetchCount, _consumerContext.ReceiveSettings.ConcurrencyLimit);

            _consumer.Subscribe(_consumerContext.ReceiveSettings.Topic);

            SetReady();

            try
            {
                while (!IsStopping)
                {
                    ConsumeResult<TKey, TValue> consumeResult = _consumer.Consume(_cancellationTokenSource.Token);
                    await executor.Push(() => Handle(consumeResult), Stopping);
                }
            }
            catch (OperationCanceledException exception) when (exception.CancellationToken == Stopping)
            {
            }
            catch (Exception exception)
            {
                LogContext.Error?.Log(exception, "Consume Loop faulted");
            }
            finally
            {
                _consumer.Unsubscribe();
                _consumer.Dispose();
                await executor.DisposeAsync().ConfigureAwait(false);
            }

            SetCompleted(TaskUtil.Completed);
        }

        async Task Handle(ConsumeResult<TKey, TValue> result)
        {
            if (IsStopping)
                return;

            var context = new ConsumeResultReceiveContext<TKey, TValue>(result, _context, _consumerLockContext, _consumerContext.HeadersDeserializer);

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

        void HandleKafkaError(Error error)
        {
            var activeDispatchCount = _dispatcher.ActiveDispatchCount;
            LogContext.Error?.Log("Consumer error ({Code}): {Reason} on {Topic}", error.Code, error.Reason, _consumerContext.ReceiveSettings.Topic);
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

            return base.StopAgent(context);
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
        }

        public long DeliveryCount => _dispatcher.DispatchCount;

        public int ConcurrentDeliveryCount => _dispatcher.MaxConcurrentDispatchCount;
    }
}
