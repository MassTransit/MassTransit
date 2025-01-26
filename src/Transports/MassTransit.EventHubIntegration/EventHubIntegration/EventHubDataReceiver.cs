namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs;
    using Azure.Messaging.EventHubs.Processor;
    using Checkpoints;
    using MassTransit.Middleware;
    using Transports;
    using Util;


    public class EventHubDataReceiver :
        ConsumerAgent<PartitionOffset>,
        IEventHubDataReceiver
    {
        readonly CancellationTokenSource _checkpointTokenSource;
        readonly EventProcessorClient _client;
        readonly ReceiveEndpointContext _context;
        readonly IChannelExecutorPool<ProcessEventArgs> _executorPool;
        readonly SemaphoreSlim _limit;
        readonly IProcessorLockContext _lockContext;

        public EventHubDataReceiver(ReceiveSettings receiveSettings, ReceiveEndpointContext context, ProcessorContext processorContext)
            : base(context)
        {
            _context = context;
            _checkpointTokenSource = CancellationTokenSource.CreateLinkedTokenSource(Stopped);
            _limit = new SemaphoreSlim(receiveSettings.PrefetchCount);

            var lockContext = new ProcessorLockContext(processorContext, receiveSettings, _checkpointTokenSource.Token);

            IHashGenerator hashGenerator = new Murmur3UnsafeHashGenerator();
            _executorPool = new PartitionChannelExecutorPool<ProcessEventArgs>(GetBytes, hashGenerator,
                receiveSettings.ConcurrentMessageLimit,
                receiveSettings.ConcurrentDeliveryLimit);

            _client = processorContext.GetClient(lockContext);
            _lockContext = lockContext;

            _client.ProcessErrorAsync += HandleError;
            _client.ProcessEventAsync += HandleMessage;

            TrySetManualConsumeTask();

            SetReady(_client.StartProcessingAsync(Stopping));
        }

        async Task HandleError(ProcessErrorEventArgs eventArgs)
        {
            LogContext.SetCurrentIfNull(_context.LogContext);

            if (IsIdle)
            {
                LogContext.Debug?.Log("Receiver shutdown completed: {InputAddress}, PartitionId: {PartitionId}", _context.InputAddress, eventArgs.PartitionId);

                TrySetConsumeException(eventArgs.Exception);
            }
        }

        static byte[] GetBytes(ProcessEventArgs eventArgs)
        {
            var partitionKey = eventArgs.Data.PartitionKey;
            return !string.IsNullOrEmpty(partitionKey) ? Encoding.UTF8.GetBytes(partitionKey) : [];
        }

        async Task HandleMessage(ProcessEventArgs eventArgs)
        {
            if (IsStopping || !eventArgs.HasEvent)
                return;

            await _limit.WaitAsync(Stopping).ConfigureAwait(false);
            await _lockContext.Pending(eventArgs).ConfigureAwait(false);
            await _executorPool.Push(eventArgs, () => Handle(eventArgs), Stopping).ConfigureAwait(false);
        }

        async Task Handle(ProcessEventArgs eventArgs)
        {
            if (IsStopping)
                return;

            var context = new EventHubReceiveContext(eventArgs, _context);
            var cancellationToken = context.CancellationToken;
            CancellationTokenRegistration? registration = null;
            if (cancellationToken.CanBeCanceled)
                registration = cancellationToken.Register(() => _lockContext.Canceled(eventArgs, cancellationToken));

            try
            {
                await Dispatch(eventArgs, context, new EventHubReceiveLockContext(eventArgs, _lockContext)).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                context.LogTransportFaulted(exception);
            }
            finally
            {
                registration?.Dispose();
                context.Dispose();
                _limit.Release();
            }
        }

        protected override async Task ActiveAndActualAgentsCompleted(StopContext context)
        {
            var stopProcessing = _client.StopProcessingAsync();

            await base.ActiveAndActualAgentsCompleted(context).ConfigureAwait(false);

            await _executorPool.DisposeAsync().ConfigureAwait(false);

            // There is not point to wait any longer, we drained our queue
            _checkpointTokenSource.Cancel();

            await stopProcessing.ConfigureAwait(false);
            _client.ProcessEventAsync -= HandleMessage;
            _client.ProcessErrorAsync -= HandleError;

            await _lockContext.DisposeAsync().ConfigureAwait(false);
            _checkpointTokenSource.Dispose();
            _limit.Dispose();
        }
    }
}
