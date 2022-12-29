namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs;
    using Azure.Messaging.EventHubs.Processor;
    using Internals;
    using MassTransit.Middleware;
    using Transports;
    using Util;


    public class EventHubDataReceiver :
        Agent,
        IEventHubDataReceiver
    {
        readonly ReceiveEndpointContext _context;
        readonly TaskCompletionSource<bool> _deliveryComplete;
        readonly IReceivePipeDispatcher _dispatcher;
        readonly ProcessorContext _processorContext;
        readonly EventProcessorClient _client;
        readonly IChannelExecutorPool<ProcessEventArgs> _executorPool;

        public EventHubDataReceiver(ReceiveSettings receiveSettings, ReceiveEndpointContext context, ProcessorContext processorContext)
        {
            _context = context;
            _processorContext = processorContext;
            _executorPool = new CombinedChannelExecutorPool(_processorContext, receiveSettings);

            _deliveryComplete = TaskUtil.GetTask<bool>();

            _dispatcher = context.CreateReceivePipeDispatcher();
            _dispatcher.ZeroActivity += HandleDeliveryComplete;

            _client = processorContext.GetClient(HandleError);

            _client.ProcessEventAsync += HandleMessage;

            SetReady(_client.StartProcessingAsync(Stopping));
        }

        public long DeliveryCount => _dispatcher.DispatchCount;

        public int ConcurrentDeliveryCount => _dispatcher.MaxConcurrentDispatchCount;

        async Task HandleError(ProcessErrorEventArgs eventArgs)
        {
            LogContext.SetCurrentIfNull(_context.LogContext);

            var activeDispatchCount = _dispatcher.ActiveDispatchCount;
            if (activeDispatchCount == 0)
            {
                LogContext.Debug?.Log("Receiver shutdown completed: {InputAddress}, PartitionId: {PartitionId}", _context.InputAddress, eventArgs.PartitionId);

                _deliveryComplete.TrySetResult(true);

                SetCompleted(TaskUtil.Faulted<bool>(eventArgs.Exception));
            }
        }

        async Task HandleMessage(ProcessEventArgs eventArgs)
        {
            if (IsStopping || !eventArgs.HasEvent)
                return;

            await _processorContext.Pending(eventArgs).ConfigureAwait(false);
            await _executorPool.Push(eventArgs, () => Handle(eventArgs), Stopping);
        }

        async Task Handle(ProcessEventArgs eventArgs)
        {
            if (IsStopping)
                return;

            var context = new EventHubReceiveContext(eventArgs, _context, _processorContext);

            try
            {
                await _dispatcher.Dispatch(context, context).ConfigureAwait(false);
            }
            finally
            {
                context.Dispose();
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
            await _client.StopProcessingAsync().ConfigureAwait(false);

            _client.ProcessEventAsync -= HandleMessage;

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

            await _executorPool.DisposeAsync().ConfigureAwait(false);
        }


        class CombinedChannelExecutorPool :
            IChannelExecutorPool<ProcessEventArgs>
        {
            readonly IChannelExecutorPool<ProcessEventArgs> _partitionExecutorPool;
            readonly IChannelExecutorPool<ProcessEventArgs> _keyExecutorPool;

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

            public ValueTask DisposeAsync()
            {
                return _keyExecutorPool.DisposeAsync();
            }

            static byte[] GetBytes(ProcessEventArgs args)
            {
                var partitionKey = args.Data.PartitionKey;
                return !string.IsNullOrEmpty(partitionKey) ? Encoding.UTF8.GetBytes(partitionKey) : Array.Empty<byte>();
            }
        }
    }
}
