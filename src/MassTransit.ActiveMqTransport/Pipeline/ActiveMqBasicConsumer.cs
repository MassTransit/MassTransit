namespace MassTransit.ActiveMqTransport.Pipeline
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading.Tasks;
    using Apache.NMS;
    using Context;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using GreenPipes.Internals.Extensions;
    using Topology;
    using Transports.Metrics;


    /// <summary>
    /// Receives messages from ActiveMQ, pushing them to the InboundPipe of the service endpoint.
    /// </summary>
    public sealed class ActiveMqBasicConsumer :
        Supervisor,
        DeliveryMetrics
    {
        readonly TaskCompletionSource<bool> _deliveryComplete;
        readonly Uri _inputAddress;
        readonly SessionContext _session;
        readonly IMessageConsumer _messageConsumer;
        readonly ConcurrentDictionary<string, ActiveMqReceiveContext> _pending;
        readonly ReceiveSettings _receiveSettings;
        readonly ActiveMqReceiveEndpointContext _context;
        readonly IDeliveryTracker _tracker;

        /// <summary>
        /// The basic consumer receives messages pushed from the broker.
        /// </summary>
        /// <param name="session">The model context for the consumer</param>
        /// <param name="messageConsumer"></param>
        /// <param name="inputAddress">The input address for messages received by the consumer</param>
        /// <param name="context">The topology</param>
        public ActiveMqBasicConsumer(SessionContext session, IMessageConsumer messageConsumer, Uri inputAddress, ActiveMqReceiveEndpointContext context)
        {
            _session = session;
            _messageConsumer = messageConsumer;
            _inputAddress = inputAddress;
            _context = context;

            _tracker = new DeliveryTracker(HandleDeliveryComplete);

            _receiveSettings = session.GetPayload<ReceiveSettings>();

            _pending = new ConcurrentDictionary<string, ActiveMqReceiveContext>();

            _deliveryComplete = new TaskCompletionSource<bool>();

            messageConsumer.Listener += HandleMessage;

            SetReady();
        }

        async void HandleMessage(IMessage message)
        {
            if (IsStopping)
            {
                await WaitAndAbandonMessage(message).ConfigureAwait(false);
                return;
            }

            using (_tracker.BeginDelivery())
            {
                var context = new ActiveMqReceiveContext(_inputAddress, message, _context);

                context.GetOrAddPayload(() => _receiveSettings);
                context.GetOrAddPayload(() => _session);
                context.GetOrAddPayload(() => _session.ConnectionContext);

                try
                {
                    if (!_pending.TryAdd(message.NMSMessageId, context))
                        LogContext.Warning?.Log("Duplicate message: {MessageId}", message.NMSMessageId);

                    await _context.ReceiveObservers.PreReceive(context).ConfigureAwait(false);

                    await _context.ReceivePipe.Send(context).ConfigureAwait(false);

                    await context.ReceiveCompleted.ConfigureAwait(false);

                    message.Acknowledge();

                    await _context.ReceiveObservers.PostReceive(context).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await _context.ReceiveObservers.ReceiveFault(context, ex).ConfigureAwait(false);
                }
                finally
                {
                    _pending.TryRemove(message.NMSMessageId, out _);

                    context.Dispose();
                }
            }
        }

        long DeliveryMetrics.DeliveryCount => _tracker.DeliveryCount;

        int DeliveryMetrics.ConcurrentDeliveryCount => _tracker.MaxConcurrentDeliveryCount;

        void HandleDeliveryComplete()
        {
            if (IsStopping)
            {
                LogContext.Debug?.Log("Consumer shutdown completed: {InputAddress}", _context.InputAddress);

                _deliveryComplete.TrySetResult(true);
            }
        }

        async Task WaitAndAbandonMessage(IMessage message)
        {
            try
            {
                await _deliveryComplete.Task.ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                LogContext.Error?.Log(exception, "DeliveryComplete faulted during shutdown: {InputAddress}", _context.InputAddress);
            }
        }

        protected override async Task StopSupervisor(StopSupervisorContext context)
        {
            LogContext.Debug?.Log("Stopping consumer: {InputAddress}", _context.InputAddress);

            SetCompleted(ActiveAndActualAgentsCompleted(context));

            await Completed.ConfigureAwait(false);
        }

        async Task ActiveAndActualAgentsCompleted(StopSupervisorContext context)
        {
            await Task.WhenAll(context.Agents.Select(x => Completed)).UntilCompletedOrCanceled(context.CancellationToken).ConfigureAwait(false);

            if (_tracker.ActiveDeliveryCount > 0)
            {
                try
                {
                    await _deliveryComplete.Task.UntilCompletedOrCanceled(context.CancellationToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    LogContext.Warning?.Log("Stop canceled waiting for message consumers to complete: {InputAddress}", _context.InputAddress);
                }
            }

            try
            {
                _messageConsumer.Close();
                _messageConsumer.Dispose();
            }
            catch (OperationCanceledException)
            {
                LogContext.Warning?.Log("Stop canceled waiting for consumer shutdown: {InputAddress}", _context.InputAddress);
            }
        }
    }
}
