using System;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using GreenPipes.Agents;
using GreenPipes.Internals.Extensions;
using MassTransit.Context;
using MassTransit.Transports;
using MassTransit.Util;
using MassTransit.EventStoreDbIntegration.Contexts;

namespace MassTransit.EventStoreDbIntegration
{
    public class EventStoreDbDataReceiver :
        Agent,
        IEventStoreDbDataReceiver
    {
        readonly ReceiveEndpointContext _context;
        readonly TaskCompletionSource<bool> _deliveryComplete;
        readonly IReceivePipeDispatcher _dispatcher;
        readonly ChannelExecutor _executor;
        readonly ProcessorContext _processorContext;

        public EventStoreDbDataReceiver(ReceiveEndpointContext context, ProcessorContext processorContext)
        {
            _context = context;
            _processorContext = processorContext;

            _deliveryComplete = TaskUtil.GetTask<bool>();

            _dispatcher = context.CreateReceivePipeDispatcher();
            _dispatcher.ZeroActivity += HandleDeliveryComplete;

            var prefetchCount = Math.Max(1000, processorContext.ReceiveSettings.CheckpointMessageCount / 10);
            _executor = new ChannelExecutor(prefetchCount, processorContext.ReceiveSettings.ConcurrencyLimit);
        }

        public long DeliveryCount => _dispatcher.DispatchCount;

        public int ConcurrentDeliveryCount => _dispatcher.MaxConcurrentDispatchCount;

        public async Task Start()
        {
            _processorContext.ProcessEvent += HandleEvent;
            _processorContext.ProcessSubscriptionDropped += HandleSubscriptionDropped;

            await _processorContext.StartProcessingAsync(Stopping).ConfigureAwait(false);

            SetReady();
        }

        async Task HandleEvent(StreamSubscription streamSubscription, ResolvedEvent resolvedEvent, CancellationToken cancellationToken)
        {
            LogContext.SetCurrentIfNull(_context.LogContext);

            try
            {
                await _executor.Push(() => Handle(resolvedEvent), Stopping).ConfigureAwait(false);
            }
            catch (OperationCanceledException ex) when (ex.CancellationToken == Stopping)
            {

            }
        }

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        void HandleSubscriptionDropped(StreamSubscription streamSubscription, SubscriptionDroppedReason droppedReason, Exception? exception)
        {
            LogContext.SetCurrentIfNull(_context.LogContext);

            var activeDispatchCount = _dispatcher.ActiveDispatchCount;
            if (activeDispatchCount == 0)
            {
                LogContext.Debug?.Log("Receiver shutdown completed: {InputAddress}, SubscriptionId: {SubscriptionId}",
                    _context.InputAddress,
                    streamSubscription.SubscriptionId);

                _deliveryComplete.TrySetResult(true);

                if (droppedReason == SubscriptionDroppedReason.Disposed)
                    SetCompleted(TaskUtil.True);
                else
                    SetCompleted(TaskUtil.Faulted<bool>(exception));
            }
        }
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

        async Task Handle(ResolvedEvent resolvedEvent)
        {
            var context = new EventRecordReceiveContext(resolvedEvent, _context, _processorContext,
                _processorContext.ReceiveSettings.StreamCategory.IsAllStream,
                _processorContext.MetadataDeserializer);

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

            _processorContext.ProcessEvent -= HandleEvent;
            _processorContext.ProcessSubscriptionDropped -= HandleSubscriptionDropped;

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
