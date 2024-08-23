namespace MassTransit.ActiveMqTransport.Middleware
{
    using System;
    using System.Threading.Tasks;
    using Apache.NMS;
    using Apache.NMS.ActiveMQ;
    using Transports;
    using Util;


    /// <summary>
    /// Receives messages from ActiveMQ, pushing them to the InboundPipe of the service endpoint.
    /// </summary>
    public sealed class ActiveMqConsumer :
        ConsumerAgent<string>
    {
        readonly ActiveMqReceiveEndpointContext _context;
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
            : base(context, StringComparer.Ordinal)
        {
            _session = session;
            _messageConsumer = messageConsumer;
            _context = context;
            _executor = executor;

            _receiveSettings = session.GetPayload<ReceiveSettings>();

            messageConsumer.Listener += HandleMessage;

            TrySetManualConsumeTask();

            SetReady();
        }

        void HandleMessage(IMessage message)
        {
            _executor.PushWithWait(async () =>
            {
                if (IsStopping)
                    return;

                LogContext.Current = _context.LogContext;

                var context = new ActiveMqReceiveContext(message, _context, _receiveSettings, _session, _session.ConnectionContext);

                try
                {
                    await Dispatch(message.NMSMessageId, context, new ActiveMqReceiveLockContext(message)).ConfigureAwait(false);
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

        protected override async Task ActiveAndActualAgentsCompleted(StopContext context)
        {
            _messageConsumer.Stop();
            _messageConsumer.Listener -= HandleMessage;
            _messageConsumer.Start();

            await base.ActiveAndActualAgentsCompleted(context).ConfigureAwait(false);

            try
            {
                await _messageConsumer.CloseAsync().ConfigureAwait(false);
                _messageConsumer.Dispose();
            }
            catch (OperationCanceledException)
            {
                LogContext.Warning?.Log("Stop canceled waiting for consumer shutdown: {InputAddress}", _context.InputAddress);
            }
        }
    }
}
