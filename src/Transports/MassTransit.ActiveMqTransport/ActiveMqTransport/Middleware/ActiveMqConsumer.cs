namespace MassTransit.ActiveMqTransport.Middleware
{
    using System;
    using System.Threading.Tasks;
    using Apache.NMS;
    using Apache.NMS.ActiveMQ;
    using Internals;
    using MassTransit.Middleware;
    using Transports;
    using Util;


    /// <summary>
    /// Receives messages from ActiveMQ, pushing them to the InboundPipe of the service endpoint.
    /// </summary>
    public sealed class ActiveMqConsumer :
        Agent,
        DeliveryMetrics
    {
        readonly ActiveMqReceiveEndpointContext _context;
        readonly TaskCompletionSource<bool> _deliveryComplete;
        readonly IReceivePipeDispatcher _dispatcher;
        readonly ChannelExecutor _executor;
        readonly MessageConsumer _messageConsumer;
        readonly ReceiveSettings _receiveSettings;
        readonly SessionContext _session;

        /// <summary>
        /// The basic consumer receives messages pushed from the broker.
        /// </summary>
        /// <param name="session">The model context for the consumer</param>
        /// <param name="messageConsumer"></param>
        /// <param name="context">The topology</param>
        /// <param name="executor"></param>
        public ActiveMqConsumer(SessionContext session, MessageConsumer messageConsumer, ActiveMqReceiveEndpointContext context, ChannelExecutor executor)
        {
            _session = session;
            _messageConsumer = messageConsumer;
            _context = context;
            _executor = executor;

            _receiveSettings = session.GetPayload<ReceiveSettings>();

            _deliveryComplete = TaskUtil.GetTask<bool>();

            _dispatcher = context.CreateReceivePipeDispatcher();
            _dispatcher.ZeroActivity += HandleDeliveryComplete;

            messageConsumer.Listener += HandleMessage;

            SetReady();
        }

        long DeliveryMetrics.DeliveryCount => _dispatcher.DispatchCount;
        int DeliveryMetrics.ConcurrentDeliveryCount => _dispatcher.MaxConcurrentDispatchCount;

        void HandleMessage(IMessage message)
        {
            _executor.PushWithWait(async () =>
            {
                LogContext.Current = _context.LogContext;

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
            }, Stopping);
        }

        Task HandleDeliveryComplete()
        {
            if (IsStopping)
                _deliveryComplete.TrySetResult(true);

            return Task.CompletedTask;
        }

        protected override async Task StopAgent(StopContext context)
        {
            _messageConsumer.Stop();
            _messageConsumer.Listener -= HandleMessage;
            _messageConsumer.Start();

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
