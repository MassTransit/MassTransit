namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs;
    using Azure.Messaging.EventHubs.Processor;
    using MassTransit.Middleware;
    using Transports;
    using Util;


    public class EventHubDataReceiver :
        ConsumerAgent,
        IEventHubDataReceiver
    {
        readonly CancellationTokenSource _checkpointTokenSource;
        readonly EventProcessorClient _client;
        readonly ReceiveEndpointContext _context;
        readonly IChannelExecutorPool<ProcessEventArgs> _executorPool;
        readonly IProcessorLockContext _lockContext;

        public EventHubDataReceiver(ReceiveSettings receiveSettings, ReceiveEndpointContext context, ProcessorContext processorContext)
            : base(context)
        {
            _context = context;
            _checkpointTokenSource = CancellationTokenSource.CreateLinkedTokenSource(Stopped);

            var lockContext = new ProcessorLockContext(processorContext, receiveSettings, _checkpointTokenSource.Token);
            _executorPool = new CombinedChannelExecutorPool(lockContext, receiveSettings);

            _client = processorContext.GetClient(lockContext);

            _client.ProcessErrorAsync += HandleError;
            _client.ProcessEventAsync += HandleMessage;
            _lockContext = lockContext;

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

        async Task HandleMessage(ProcessEventArgs eventArgs)
        {
            if (IsStopping || !eventArgs.HasEvent)
                return;

            await _lockContext.Pending(eventArgs).ConfigureAwait(false);
            await _executorPool.Push(eventArgs, () => Handle(eventArgs), Stopping).ConfigureAwait(false);
        }

        async Task Handle(ProcessEventArgs eventArgs)
        {
            if (IsStopping)
                return;

            var context = new EventHubReceiveContext(eventArgs, _context, _lockContext);
            var cancellationToken = context.CancellationToken;
            CancellationTokenRegistration? registration = null;
            if (cancellationToken.CanBeCanceled)
                registration = cancellationToken.Register(() => _lockContext.Canceled(eventArgs, cancellationToken));

            try
            {
                await Dispatch(context, context).ConfigureAwait(false);
            }
            finally
            {
                registration?.Dispose();
                context.Dispose();
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

            _checkpointTokenSource.Dispose();
        }


        class CombinedChannelExecutorPool :
            IChannelExecutorPool<ProcessEventArgs>
        {
            readonly IChannelExecutorPool<ProcessEventArgs> _keyExecutorPool;
            readonly IChannelExecutorPool<ProcessEventArgs> _partitionExecutorPool;

            public CombinedChannelExecutorPool(IChannelExecutorPool<ProcessEventArgs> partitionExecutorPool, ReceiveSettings receiveSettings)
            {
                _partitionExecutorPool = partitionExecutorPool;
                IHashGenerator hashGenerator = new Murmur3UnsafeHashGenerator();
                _keyExecutorPool = new PartitionChannelExecutorPool<ProcessEventArgs>(GetBytes, hashGenerator,
                    receiveSettings.ConcurrentMessageLimit,
                    receiveSettings.ConcurrentDeliveryLimit);
            }

            public Task Push(ProcessEventArgs args, Func<Task> handle, CancellationToken cancellationToken)
            {
                return _partitionExecutorPool.Push(args, () => _keyExecutorPool.Run(args, handle, cancellationToken), cancellationToken);
            }

            public Task Run(ProcessEventArgs args, Func<Task> method, CancellationToken cancellationToken = default)
            {
                return _partitionExecutorPool.Run(args, () => _keyExecutorPool.Run(args, method, cancellationToken), cancellationToken);
            }

            public async ValueTask DisposeAsync()
            {
                await _partitionExecutorPool.DisposeAsync().ConfigureAwait(false);
                await _keyExecutorPool.DisposeAsync().ConfigureAwait(false);
            }

            static byte[] GetBytes(ProcessEventArgs args)
            {
                var partitionKey = args.Data.PartitionKey;
                return !string.IsNullOrEmpty(partitionKey) ? Encoding.UTF8.GetBytes(partitionKey) : Array.Empty<byte>();
            }
        }
    }
}
