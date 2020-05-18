namespace MassTransit.ActiveMqTransport.Pipeline
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Apache.NMS;
    using Context;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using GreenPipes.Internals.Extensions;
    using Topology;
    using Transports;
    using Transports.Metrics;
    using Util;


    /// <summary>
    /// Receives messages from ActiveMQ, pushing them to the InboundPipe of the service endpoint.
    /// </summary>
    public sealed class ActiveMqBasicConsumer :
        Supervisor,
        DeliveryMetrics
    {
        readonly TaskCompletionSource<bool> _deliveryComplete;
        readonly SessionContext _session;
        readonly IMessageConsumer _messageConsumer;
        readonly ReceiveSettings _receiveSettings;
        readonly ActiveMqReceiveEndpointContext _context;
        readonly IReceivePipeDispatcher _dispatcher;

        /// <summary>
        /// The basic consumer receives messages pushed from the broker.
        /// </summary>
        /// <param name="session">The model context for the consumer</param>
        /// <param name="messageConsumer"></param>
        /// <param name="context">The topology</param>
        public ActiveMqBasicConsumer(SessionContext session, IMessageConsumer messageConsumer, ActiveMqReceiveEndpointContext context)
        {
            _session = session;
            _messageConsumer = messageConsumer;
            _context = context;

            _receiveSettings = session.GetPayload<ReceiveSettings>();

            _deliveryComplete = TaskUtil.GetTask<bool>();

            _dispatcher = context.CreateReceivePipeDispatcher();
            _dispatcher.ZeroActivity += HandleDeliveryComplete;

            messageConsumer.Listener += HandleMessage;

            SetReady();
        }

        void HandleMessage(IMessage message)
        {
            Task.Run(async () =>
            {
                LogContext.Current = _context.LogContext;

                if (IsStopping)
                {
                    await WaitAndAbandonMessage().ConfigureAwait(false);
                    return;
                }

                var context = new ActiveMqReceiveContext(message, _context, _receiveSettings, _session, _session.ConnectionContext);

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
            });
        }

        long DeliveryMetrics.DeliveryCount => _dispatcher.DispatchCount;
        int DeliveryMetrics.ConcurrentDeliveryCount => _dispatcher.MaxConcurrentDispatchCount;

        Task HandleDeliveryComplete()
        {
            if (IsStopping)
            {
                LogContext.Debug?.Log("Consumer shutdown completed: {InputAddress}", _context.InputAddress);

                _deliveryComplete.TrySetResult(true);
            }

            return TaskUtil.Completed;
        }

        async Task WaitAndAbandonMessage()
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
            await Task.WhenAll(context.Agents.Select(x => Completed)).OrCanceled(context.CancellationToken).ConfigureAwait(false);

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
