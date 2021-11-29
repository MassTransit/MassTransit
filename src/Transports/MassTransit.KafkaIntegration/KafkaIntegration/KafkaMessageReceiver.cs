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


    public class KafkaMessageReceiver<TKey, TValue> :
        Agent,
        IKafkaMessageReceiver<TKey, TValue>
        where TValue : class
    {
        readonly CancellationTokenSource _cancellationTokenSource;
        readonly ConsumerContext<TKey, TValue> _consumerContext;
        readonly ReceiveEndpointContext _context;
        readonly TaskCompletionSource<bool> _deliveryComplete;
        readonly IReceivePipeDispatcher _dispatcher;

        public KafkaMessageReceiver(ReceiveEndpointContext context, ConsumerContext<TKey, TValue> consumerContext)
        {
            _context = context;
            _consumerContext = consumerContext;
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(Stopping);

            _consumerContext.ErrorHandler += HandleKafkaError;

            _deliveryComplete = TaskUtil.GetTask<bool>();

            _dispatcher = context.CreateReceivePipeDispatcher();
            _dispatcher.ZeroActivity += HandleDeliveryComplete;

            Task.Run(Consume);
        }

        public long DeliveryCount => _dispatcher.DispatchCount;

        public int ConcurrentDeliveryCount => _dispatcher.MaxConcurrentDispatchCount;

        async Task Consume()
        {
            var prefetchCount = Math.Max(1000, _consumerContext.ReceiveSettings.CheckpointMessageCount / 10);
            var executor = new ChannelExecutor(prefetchCount, _consumerContext.ReceiveSettings.ConcurrencyLimit);

            await _consumerContext.Subscribe().ConfigureAwait(false);

            SetReady();

            try
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    ConsumeResult<TKey, TValue> consumeResult = await _consumerContext.Consume(_cancellationTokenSource.Token).ConfigureAwait(false);
                    await executor.Push(() => Handle(consumeResult), Stopping).ConfigureAwait(false);
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
            finally
            {
                await executor.DisposeAsync().ConfigureAwait(false);
            }
        }

        async Task Handle(ConsumeResult<TKey, TValue> result)
        {
            if (IsStopping)
                return;

            var context = new KafkaReceiveContext<TKey, TValue>(result, _context, _consumerContext, _consumerContext.HeadersDeserializer);

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

        void HandleKafkaError(IConsumer<TKey, TValue> consumer, Error error)
        {
            EnabledLogger? logger = error.IsFatal ? LogContext.Error : LogContext.Warning;
            logger?.Log("Consumer error ({Code}): {Reason} on {Topic}", error.Code, error.Reason, _consumerContext.ReceiveSettings.Topic);

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

        protected override async Task StopAgent(StopContext context)
        {
            await _consumerContext.Close().ConfigureAwait(false);

            _consumerContext.ErrorHandler -= HandleKafkaError;

            LogContext.Debug?.Log("Stopping consumer: {InputAddress}", _context.InputAddress);

            SetCompleted(ActiveAndActualAgentsCompleted(context));

            await Completed.ConfigureAwait(false);
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
    }
}
