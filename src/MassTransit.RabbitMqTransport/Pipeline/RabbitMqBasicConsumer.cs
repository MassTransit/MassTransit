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
        IBasicConsumer,
        RabbitMqDeliveryMetrics
    {
        readonly TaskCompletionSource<bool> _deliveryComplete;
        readonly ModelContext _model;
        readonly ConcurrentDictionary<ulong, RabbitMqReceiveContext> _pending;
        readonly ReceiveSettings _receiveSettings;
        readonly RabbitMqReceiveEndpointContext _context;
        readonly IReceivePipeDispatcher _dispatcher;

        string _consumerTag;

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
        }

        /// <summary>
        /// Called when the consumer is ready to be delivered messages by the broker
        /// </summary>
        /// <param name="consumerTag"></param>
        void IBasicConsumer.HandleBasicConsumeOk(string consumerTag)
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
        void IBasicConsumer.HandleBasicCancelOk(string consumerTag)
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
        void IBasicConsumer.HandleBasicCancel(string consumerTag)
        {
            LogContext.Current = _context.LogContext;

            LogContext.Debug?.Log("Consumer Canceled: {InputAddress} - {ConsumerTag}", _context.InputAddress, consumerTag);

            foreach (var context in _pending.Values)
                context.Cancel();

            ConsumerCancelled?.Invoke(this, new ConsumerEventArgs(consumerTag));

            _deliveryComplete.TrySetResult(true);
            SetCompleted(TaskUtil.Completed);
        }

        void IBasicConsumer.HandleModelShutdown(object model, ShutdownEventArgs reason)
        {
            LogContext.Current = _context.LogContext;

            LogContext.Debug?.Log(
                "Consumer Model Shutdown: {InputAddress} - {ConsumerTag}, Concurrent Peak: {MaxConcurrentDeliveryCount}, {ReplyCode}-{ReplyText}",
                _context.InputAddress, _consumerTag, _dispatcher.MaxConcurrentDispatchCount, reason.ReplyCode, reason.ReplyText);

            _deliveryComplete.TrySetResult(false);
            SetCompleted(TaskUtil.Completed);
        }

        void IBasicConsumer.HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey,
            IBasicProperties properties, byte[] body)
        {
            Task.Run(async () =>
            {
                LogContext.Current = _context.LogContext;

                if (IsStopping && _receiveSettings.NoAck == false)
                {
                    await WaitAndAbandonMessage(deliveryTag).ConfigureAwait(false);
                    return;
                }

                var context = new RabbitMqReceiveContext(exchange, routingKey, _consumerTag, deliveryTag, body, redelivered, properties, _context,
                    _receiveSettings,
                    _model, _model.ConnectionContext);

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

        IModel IBasicConsumer.Model => _model.Model;

        public event EventHandler<ConsumerEventArgs> ConsumerCancelled;

        string RabbitMqDeliveryMetrics.ConsumerTag => _consumerTag;

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
                {
                    pendingContext.Cancel();
                }

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
