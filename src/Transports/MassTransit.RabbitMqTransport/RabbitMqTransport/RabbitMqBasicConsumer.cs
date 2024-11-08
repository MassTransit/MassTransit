namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using Transports;


    /// <summary>
    /// Receives messages from RabbitMQ, pushing them to the InboundPipe of the service endpoint.
    /// </summary>
    public class RabbitMqBasicConsumer :
        ConsumerAgent<ulong>,
        IAsyncBasicConsumer,
        RabbitMqDeliveryMetrics
    {
        readonly ChannelContext _channel;
        readonly RabbitMqReceiveEndpointContext _context;
        readonly SemaphoreSlim _limit;
        readonly ReceiveSettings _receiveSettings;

        string _consumerTag;
        AsyncEventHandler<ConsumerEventArgs> _onConsumerCancelled;

        /// <summary>
        /// The basic consumer receives messages pushed from the broker.
        /// </summary>
        /// <param name="channel">The channel context for the consumer</param>
        /// <param name="context">The topology</param>
        public RabbitMqBasicConsumer(ChannelContext channel, RabbitMqReceiveEndpointContext context)
            : base(context)
        {
            _channel = channel;
            _context = context;

            _receiveSettings = channel.GetPayload<ReceiveSettings>();

            if (context.ConcurrentMessageLimit.HasValue)
                _limit = new SemaphoreSlim(context.ConcurrentMessageLimit.Value);

            TrySetManualConsumeTask();
        }

        public Task HandleBasicConsumeOkAsync(string consumerTag, CancellationToken cancellationToken)
        {
            LogContext.Current = _context.LogContext;

            LogContext.Debug?.Log("Consumer Ok: {InputAddress} - {ConsumerTag}", _context.InputAddress, consumerTag);

            _consumerTag = consumerTag;

            SetReady();

            return Task.CompletedTask;
        }

        public Task HandleBasicCancelOkAsync(string consumerTag, CancellationToken cancellationToken)
        {
            LogContext.Current = _context.LogContext;

            LogContext.Debug?.Log("Consumer Cancel Ok: {InputAddress} - {ConsumerTag}", _context.InputAddress, consumerTag);

            TrySetConsumeCompleted();

            return Task.CompletedTask;
        }

        public async Task HandleBasicCancelAsync(string consumerTag, CancellationToken cancellationToken)
        {
            LogContext.Current = _context.LogContext;

            LogContext.Debug?.Log("Consumer Canceled: {InputAddress} - {ConsumerTag}", _context.InputAddress, consumerTag);

            if (_onConsumerCancelled != null)
                await _onConsumerCancelled(this, new ConsumerEventArgs([consumerTag])).ConfigureAwait(false);

            TrySetConsumeCanceled(cancellationToken);
        }

        public Task HandleChannelShutdownAsync(object channel, ShutdownEventArgs reason)
        {
            LogContext.Current = _context.LogContext;

            LogContext.Debug?.Log(
                "Consumer Channel Shutdown: {InputAddress} - {ConsumerTag}, Concurrent Peak: {MaxConcurrentDeliveryCount}, {ReplyCode}-{ReplyText}",
                _context.InputAddress, _consumerTag, ConcurrentDeliveryCount, reason.ReplyCode, reason.ReplyText);

            TrySetConsumeCanceled();

            return Task.CompletedTask;
        }

        public async Task HandleBasicDeliverAsync(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey,
            IReadOnlyBasicProperties properties, ReadOnlyMemory<byte> body, CancellationToken cancellationToken)
        {
            try
            {
                if (_limit != null)
                    await _limit.WaitAsync(Stopping).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            var bodyBytes = body.ToArray();

            LogContext.Current = _context.LogContext;

            var context = new RabbitMqReceiveContext(exchange, routingKey, _consumerTag, deliveryTag, bodyBytes, redelivered, properties,
                _context, _receiveSettings, _channel, _channel.ConnectionContext);

            try
            {
                if (IsStopping)
                    return;

                await Dispatch(deliveryTag, context,
                        _receiveSettings.NoAck ? NoLockReceiveContext.Instance : new RabbitMqReceiveLockContext(_channel, deliveryTag))
                    .ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                context.LogTransportFaulted(exception);
            }
            finally
            {
                _limit?.Release();

                context.Dispose();
            }
        }

        public IChannel Channel => _channel.Channel;

        string RabbitMqDeliveryMetrics.ConsumerTag => _consumerTag;

        public event AsyncEventHandler<ConsumerEventArgs> ConsumerCancelled
        {
            add => _onConsumerCancelled += value;
            remove => _onConsumerCancelled -= value;
        }

        protected override bool IsTrackable(ulong deliveryTag)
        {
            return deliveryTag != 1 || _context.IsNotReplyTo;
        }

        protected override async Task StopAgent(StopContext context)
        {
            try
            {
                await base.StopAgent(context).ConfigureAwait(false);
            }
            finally
            {
                _limit?.Dispose();
            }
        }

        protected override async Task ActiveAndActualAgentsCompleted(StopContext context)
        {
            try
            {
                if (IsGracefulShutdown && _channel.Channel.IsOpen)
                    await _channel.BasicCancel(_consumerTag).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                LogContext.Warning?.Log(exception, "BasicCancel faulted: {InputAddress} - {ConsumerTag}", _context.InputAddress, _consumerTag);
            }

            await base.ActiveAndActualAgentsCompleted(context).ConfigureAwait(false);
        }
    }
}
