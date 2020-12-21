namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs;
    using Azure.Messaging.EventHubs.Processor;
    using Context;
    using Contexts;
    using GreenPipes.Agents;
    using GreenPipes.Internals.Extensions;
    using Transports;
    using Util;


    public class EventHubDataReceiver :
        Agent,
        IEventHubDataReceiver
    {
        readonly ReceiveEndpointContext _context;
        readonly IProcessorLockContext _lockContext;
        readonly IReceivePipeDispatcher _dispatcher;
        readonly EventProcessorClient _client;
        readonly TaskCompletionSource<bool> _deliveryComplete;
        readonly ChannelExecutor _executor;

        public EventHubDataReceiver(ReceiveEndpointContext context, IEventHubProcessorContext processorContext)
        {
            _context = context;
            _dispatcher = context.CreateReceivePipeDispatcher();

            _client = processorContext.CreateClient();

            _lockContext = new ProcessorLockContext(_client, context.LogContext, processorContext);

            _deliveryComplete = TaskUtil.GetTask<bool>();

            _dispatcher = context.CreateReceivePipeDispatcher();
            _dispatcher.ZeroActivity += HandleDeliveryComplete;

            _client.ProcessEventAsync += HandleMessage;
            _client.ProcessErrorAsync += HandleError;

            var prefetchCount = Math.Max(1000, processorContext.ReceiveSettings.CheckpointMessageCount / 10);
            _executor = new ChannelExecutor(prefetchCount, processorContext.ReceiveSettings.ConcurrencyLimit);
        }

        Task HandleMessage(ProcessEventArgs eventArgs)
        {
            LogContext.SetCurrentIfNull(_context.LogContext);

            try
            {
                return _executor.Push(() => Handle(eventArgs), Stopping);
            }
            catch (OperationCanceledException e) when (e.CancellationToken == Stopping)
            {
            }

            return TaskUtil.Completed;
        }

        Task HandleError(ProcessErrorEventArgs eventArgs)
        {
            LogContext.SetCurrentIfNull(_context.LogContext);

            var activeDispatchCount = _dispatcher.ActiveDispatchCount;
            if (activeDispatchCount == 0)
            {
                LogContext.Debug?.Log("Receiver shutdown completed: {InputAddress}, PartitionId: {PartitionId}", _context.InputAddress, eventArgs.PartitionId);

                _deliveryComplete.TrySetResult(true);

                SetCompleted(TaskUtil.Faulted<bool>(eventArgs.Exception));
            }

            return TaskUtil.Completed;
        }

        async Task Handle(ProcessEventArgs eventArgs)
        {
            if (!eventArgs.HasEvent)
                return;

            var context = new EventDataReceiveContext(eventArgs, _context, _lockContext);

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
            await _client.StopProcessingAsync(context.CancellationToken).ConfigureAwait(false);

            await _executor.DisposeAsync().ConfigureAwait(false);

            LogContext.Debug?.Log("Stopping consumer: {InputAddress}", _context.InputAddress);

            SetCompleted(ActiveAndActualAgentsCompleted(context));

            await base.StopAgent(context).ConfigureAwait(false);
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

        public async Task Start()
        {
            await _client.StartProcessingAsync(Stopping).ConfigureAwait(false);

            SetReady();
        }
    }
}
