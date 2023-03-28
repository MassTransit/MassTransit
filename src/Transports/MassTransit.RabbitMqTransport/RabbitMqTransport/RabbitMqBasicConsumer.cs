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
        IBasicConsumer,
        RabbitMqDeliveryMetrics
    {
        readonly RabbitMqReceiveEndpointContext _context;
        readonly SemaphoreSlim _limit;

        readonly ModelContext _model;
        readonly ReceiveSettings _receiveSettings;

        string _consumerTag;

        EventHandler<ConsumerEventArgs> _onConsumerCancelled;

        /// <summary>
        /// The basic consumer receives messages pushed from the broker.
        /// </summary>
        /// <param name="model">The model context for the consumer</param>
        /// <param name="context">The topology</param>
        public RabbitMqBasicConsumer(ModelContext model, RabbitMqReceiveEndpointContext context)
            : base(context)
        {
            _model = model;
            _context = context;

            _receiveSettings = model.GetPayload<ReceiveSettings>();

            if (context.ConcurrentMessageLimit.HasValue)
                _limit = new SemaphoreSlim(context.ConcurrentMessageLimit.Value);

            ConsumerCancelled += OnConsumerCancelled;

            TrySetManualConsumeTask();
        }

        /// <summary>
        /// Called when the consumer is ready to be delivered messages by the broker
        /// </summary>
        /// <param name="consumerTag"></param>
        Task IAsyncBasicConsumer.HandleBasicConsumeOk(string consumerTag)
        {
            HandleBasicConsumeOk(consumerTag);

            return Task.CompletedTask;
        }

        Task IAsyncBasicConsumer.HandleBasicCancelOk(string consumerTag)
        {
            HandleBasicCancelOk(consumerTag);

            return Task.CompletedTask;
        }

        Task IAsyncBasicConsumer.HandleBasicCancel(string consumerTag)
        {
            HandleBasicCancel(consumerTag);

            return Task.CompletedTask;
        }

        Task IAsyncBasicConsumer.HandleModelShutdown(object model, ShutdownEventArgs reason)
        {
            HandleModelShutdown(_model, reason);

            return Task.CompletedTask;
        }

        Task IAsyncBasicConsumer.HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey,
            IBasicProperties properties, ReadOnlyMemory<byte> body)
        {
            HandleBasicDeliver(consumerTag, deliveryTag, redelivered, exchange, routingKey, properties, body);

            return Task.CompletedTask;
        }

        public IModel Model => _model.Model;

        public event AsyncEventHandler<ConsumerEventArgs> ConsumerCancelled;

        /// <summary>
        /// Called when the consumer is ready to be delivered messages by the broker
        /// </summary>
        /// <param name="consumerTag"></param>
        public void HandleBasicConsumeOk(string consumerTag)
        {
            LogContext.Current = _context.LogContext;

            LogContext.Debug?.Log("Consumer Ok: {InputAddress} - {ConsumerTag}", _context.InputAddress, consumerTag);

            _consumerTag = consumerTag;

            SetReady();
        }

        /// <summary>
        /// Called when the broker has received and acknowledged the BasicCancel, indicating
        /// that the consumer is requesting to be shut down gracefully.
        /// </summary>
        /// <param name="consumerTag">The consumerTag that was shut down.</param>
        public void HandleBasicCancelOk(string consumerTag)
        {
            LogContext.Current = _context.LogContext;

            LogContext.Debug?.Log("Consumer Cancel Ok: {InputAddress} - {ConsumerTag}", _context.InputAddress, consumerTag);

            TrySetConsumeCompleted();
        }

        /// <summary>
        /// Called when the broker cancels the consumer due to an unexpected event, such as a
        /// queue removal, or other change, that would disconnect the consumer.
        /// </summary>
        /// <param name="consumerTag">The consumerTag that is being cancelled.</param>
        public void HandleBasicCancel(string consumerTag)
        {
            LogContext.Current = _context.LogContext;

            LogContext.Debug?.Log("Consumer Canceled: {InputAddress} - {ConsumerTag}", _context.InputAddress, consumerTag);

            ConsumerCancelled?.Invoke(this, new ConsumerEventArgs(new[] { consumerTag }));

            TrySetConsumeCanceled();
        }

        public void HandleModelShutdown(object model, ShutdownEventArgs reason)
        {
            LogContext.Current = _context.LogContext;

            LogContext.Debug?.Log(
                "Consumer Model Shutdown: {InputAddress} - {ConsumerTag}, Concurrent Peak: {MaxConcurrentDeliveryCount}, {ReplyCode}-{ReplyText}",
                _context.InputAddress, _consumerTag, ConcurrentDeliveryCount, reason.ReplyCode, reason.ReplyText);

            TrySetConsumeCanceled();
        }

        public void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey,
            IBasicProperties properties, ReadOnlyMemory<byte> body)
        {
            var bodyBytes = body.ToArray();

            Task.Run(async () =>
            {
                LogContext.Current = _context.LogContext;

                var context = new RabbitMqReceiveContext(exchange, routingKey, _consumerTag, deliveryTag, bodyBytes, redelivered, properties,
                    _context, _receiveSettings, _model, _model.ConnectionContext);

                if (_limit != null)
                    await _limit.WaitAsync(context.CancellationToken).ConfigureAwait(false);

                try
                {
                    await Dispatch(deliveryTag, context,
                            _ => _receiveSettings.NoAck ? NoLockReceiveContext.Instance : new RabbitMqReceiveLockContext(_model, deliveryTag))
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
            });
        }

        event EventHandler<ConsumerEventArgs> IBasicConsumer.ConsumerCancelled
        {
            add => _onConsumerCancelled += value;
            remove => _onConsumerCancelled -= value;
        }

        string RabbitMqDeliveryMetrics.ConsumerTag => _consumerTag;

        protected override bool IsTrackable(ulong deliveryTag)
        {
            return deliveryTag != 1 || _context.IsNotReplyTo;
        }

        Task OnConsumerCancelled(object obj, ConsumerEventArgs args)
        {
            _onConsumerCancelled?.Invoke(obj, args);

            return Task.CompletedTask;
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
                if (IsGracefulShutdown)
                    await _model.BasicCancel(_consumerTag).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                LogContext.Warning?.Log(exception, "BasicCancel faulted: {InputAddress} - {ConsumerTag}", _context.InputAddress, _consumerTag);
            }

            await base.ActiveAndActualAgentsCompleted(context).ConfigureAwait(false);
        }
    }
}
