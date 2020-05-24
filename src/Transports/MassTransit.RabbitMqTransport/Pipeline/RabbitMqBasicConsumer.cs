namespace MassTransit.RabbitMqTransport.Pipeline
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using GreenPipes.Internals.Extensions;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using Topology;
    using Transports;
    using Transports.Metrics;
    using Util;


    /// <summary>
    /// Receives messages from RabbitMQ, pushing them to the InboundPipe of the service endpoint.
    /// </summary>
    public class RabbitMqBasicConsumer :
        Supervisor,
        IAsyncBasicConsumer,
        IBasicConsumer,
        RabbitMqDeliveryMetrics
    {
        readonly RabbitMqReceiveEndpointContext _context;
        readonly TaskCompletionSource<bool> _deliveryComplete;
        readonly IReceivePipeDispatcher _dispatcher;
        readonly ModelContext _model;
        readonly ConcurrentDictionary<ulong, RabbitMqReceiveContext> _pending;
        readonly ReceiveSettings _receiveSettings;

        string _consumerTag;

        EventHandler<ConsumerEventArgs> _onConsumerCancelled;

        /// <summary>
        /// The basic consumer receives messages pushed from the broker.
        /// </summary>
        /// <param name="model">The model context for the consumer</param>
        /// <param name="context">The topology</param>
        public RabbitMqBasicConsumer(ModelContext model, RabbitMqReceiveEndpointContext context)
        {
            _model = model;
            _context = context;

            _receiveSettings = model.GetPayload<ReceiveSettings>();

            _pending = new ConcurrentDictionary<ulong, RabbitMqReceiveContext>();

            _dispatcher = context.CreateReceivePipeDispatcher();
            _dispatcher.ZeroActivity += HandleDeliveryComplete;

            _deliveryComplete = TaskUtil.GetTask<bool>();

            ConsumerCancelled += OnConsumerCancelled;
        }

        /// <summary>
        /// Called when the consumer is ready to be delivered messages by the broker
        /// </summary>
        /// <param name="consumerTag"></param>
        Task IAsyncBasicConsumer.HandleBasicConsumeOk(string consumerTag)
        {
            HandleBasicConsumeOk(consumerTag);

            return TaskUtil.Completed;
        }

        Task IAsyncBasicConsumer.HandleBasicCancelOk(string consumerTag)
        {
            HandleBasicCancelOk(consumerTag);

            return TaskUtil.Completed;
        }

        Task IAsyncBasicConsumer.HandleBasicCancel(string consumerTag)
        {
            HandleBasicCancel(consumerTag);

            return TaskUtil.Completed;
        }

        Task IAsyncBasicConsumer.HandleModelShutdown(object model, ShutdownEventArgs reason)
        {
            HandleModelShutdown(_model, reason);

            return TaskUtil.Completed;
        }

        Task IAsyncBasicConsumer.HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey,
            IBasicProperties properties, ReadOnlyMemory<byte> body)
        {
            HandleBasicDeliver(consumerTag, deliveryTag, redelivered, exchange, routingKey, properties, body);

            return TaskUtil.Completed;
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

            _deliveryComplete.TrySetResult(true);
            SetCompleted(TaskUtil.Completed);
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

            foreach (var context in _pending.Values)
                context.Cancel();

            ConsumerCancelled?.Invoke(this, new ConsumerEventArgs(new[] {consumerTag}));

            _deliveryComplete.TrySetResult(true);
            SetCompleted(TaskUtil.Completed);
        }

        public void HandleModelShutdown(object model, ShutdownEventArgs reason)
        {
            LogContext.Current = _context.LogContext;

            LogContext.Debug?.Log(
                "Consumer Model Shutdown: {InputAddress} - {ConsumerTag}, Concurrent Peak: {MaxConcurrentDeliveryCount}, {ReplyCode}-{ReplyText}",
                _context.InputAddress, _consumerTag, _dispatcher.MaxConcurrentDispatchCount, reason.ReplyCode, reason.ReplyText);

            _deliveryComplete.TrySetResult(false);
            SetCompleted(TaskUtil.Completed);
        }

        public void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey,
            IBasicProperties properties, ReadOnlyMemory<byte> body)
        {
            var bodyBytes = body.ToArray();

            Task.Run(async () =>
            {
                LogContext.Current = _context.LogContext;

                if (IsStopping && _receiveSettings.NoAck == false)
                {
                    await WaitAndAbandonMessage(deliveryTag).ConfigureAwait(false);
                    return;
                }

                var context = new RabbitMqReceiveContext(exchange, routingKey, _consumerTag, deliveryTag, bodyBytes, redelivered, properties,
                    _context, _receiveSettings, _model, _model.ConnectionContext);

                if (!_pending.TryAdd(deliveryTag, context))
                    LogContext.Warning?.Log("Duplicate BasicDeliver: {DeliveryTag}", deliveryTag);

                var receiveLock = _receiveSettings.NoAck ? default : new RabbitMqReceiveLockContext(_model, deliveryTag);

                try
                {
                    await _dispatcher.Dispatch(context, receiveLock).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    context.LogTransportFaulted(exception);
                }
                finally
                {
                    _pending.TryRemove(deliveryTag, out _);
                    context.Dispose();
                }
            });
        }

        string RabbitMqDeliveryMetrics.ConsumerTag => _consumerTag;

        long DeliveryMetrics.DeliveryCount => _dispatcher.DispatchCount;

        int DeliveryMetrics.ConcurrentDeliveryCount => _dispatcher.MaxConcurrentDispatchCount;

        event EventHandler<ConsumerEventArgs> IBasicConsumer.ConsumerCancelled
        {
            add => _onConsumerCancelled += value;
            remove => _onConsumerCancelled -= value;
        }

        Task OnConsumerCancelled(object obj, ConsumerEventArgs args)
        {
            _onConsumerCancelled?.Invoke(obj, args);

            return TaskUtil.Completed;
        }

        Task HandleDeliveryComplete()
        {
            if (IsStopping)
            {
                LogContext.Debug?.Log("Consumer shutdown completed: {InputAddress}", _context.InputAddress);

                _deliveryComplete.TrySetResult(true);
            }

            return TaskUtil.Completed;
        }

        async Task WaitAndAbandonMessage(ulong deliveryTag)
        {
            try
            {
                await _deliveryComplete.Task.ConfigureAwait(false);

                _model.BasicNack(deliveryTag, false, true);
            }
            catch (Exception exception)
            {
                LogContext.Error?.Log(exception, "Message NACK faulted during shutdown: {InputAddress} - {ConsumerTag}", _context.InputAddress,
                    _consumerTag);
            }
        }

        protected override async Task StopSupervisor(StopSupervisorContext context)
        {
            LogContext.Debug?.Log("Stopping Consumer: {InputAddress} - {ConsumerTag}", _context.InputAddress, _consumerTag);

            SetCompleted(ActiveAndActualAgentsCompleted(context));

            try
            {
                await Completed.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                foreach (var pendingContext in _pending.Values)
                    pendingContext.Cancel();

                throw;
            }
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
                    LogContext.Warning?.Log("Stop canceled waiting for message consumers to complete: {InputAddress} - {ConsumerTag}",
                        _context.InputAddress, _consumerTag);
                }
            }

            try
            {
                await _model.BasicCancel(_consumerTag).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                LogContext.Warning?.Log("Stop canceled waiting for consumer cancellation: {InputAddress} - {ConsumerTag}", _context.InputAddress,
                    _consumerTag);
            }
        }
    }
}
