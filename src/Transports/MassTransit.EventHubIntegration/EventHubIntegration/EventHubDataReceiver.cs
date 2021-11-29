namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Threading.Tasks;
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
        readonly ChannelExecutor _executor;
        readonly ProcessorContext _processorContext;

        public EventHubDataReceiver(ReceiveEndpointContext context, ProcessorContext processorContext)
        {
            _context = context;
            _processorContext = processorContext;

            _deliveryComplete = TaskUtil.GetTask<bool>();

            _dispatcher = context.CreateReceivePipeDispatcher();
            _dispatcher.ZeroActivity += HandleDeliveryComplete;

            var prefetchCount = Math.Max(1000, processorContext.ReceiveSettings.CheckpointMessageCount / 10);

            _executor = new ChannelExecutor(prefetchCount, processorContext.ReceiveSettings.ConcurrentMessageLimit);
        }

        public long DeliveryCount => _dispatcher.DispatchCount;

        public int ConcurrentDeliveryCount => _dispatcher.MaxConcurrentDispatchCount;

        public async Task Start()
        {
            _processorContext.ProcessEvent += HandleMessage;
            _processorContext.ProcessError += HandleError;

            await _processorContext.StartProcessingAsync(Stopping).ConfigureAwait(false);

            SetReady();
        }

        async Task HandleMessage(ProcessEventArgs eventArgs)
        {
            LogContext.SetCurrentIfNull(_context.LogContext);

            try
            {
                await _executor.Push(() => Handle(eventArgs), Stopping).ConfigureAwait(false);
            }
            catch (OperationCanceledException e) when (e.CancellationToken == Stopping)
            {
            }
        }

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

        async Task Handle(ProcessEventArgs eventArgs)
        {
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
            await _processorContext.StopProcessingAsync().ConfigureAwait(false);

            _processorContext.ProcessEvent -= HandleMessage;
            _processorContext.ProcessError -= HandleError;

            await _executor.DisposeAsync().ConfigureAwait(false);

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
