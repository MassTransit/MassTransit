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
    using Logging;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using Topology;
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
        readonly Uri _inputAddress;
        readonly ModelContext _model;
        readonly ConcurrentDictionary<ulong, RabbitMqReceiveContext> _pending;
        readonly ReceiveSettings _receiveSettings;
        readonly RabbitMqReceiveEndpointContext _context;
        readonly IDeliveryTracker _tracker;

        string _consumerTag;

        /// <summary>
        /// The basic consumer receives messages pushed from the broker.
        /// </summary>
        /// <param name="model">The model context for the consumer</param>
        /// <param name="inputAddress">The input address for messages received by the consumer</param>
        /// <param name="context">The topology</param>
        public RabbitMqBasicConsumer(ModelContext model, Uri inputAddress, RabbitMqReceiveEndpointContext context)
        {
            _model = model;
            _inputAddress = inputAddress;
            _context = context;

            _tracker = new DeliveryTracker(HandleDeliveryComplete);

            _receiveSettings = model.GetPayload<ReceiveSettings>();

            _pending = new ConcurrentDictionary<ulong, RabbitMqReceiveContext>();

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
                _context.InputAddress, _consumerTag, _tracker.MaxConcurrentDeliveryCount, reason.ReplyCode, reason.ReplyText);

            _deliveryComplete.TrySetResult(false);
            SetCompleted(TaskUtil.Completed);
        }

        async void IBasicConsumer.HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange,
            string routingKey,
            IBasicProperties properties, byte[] body)
        {
            LogContext.Current = _context.LogContext;

            if (IsStopping)
            {
                await WaitAndAbandonMessage(deliveryTag).ConfigureAwait(false);
                return;
            }

            var delivery = _tracker.BeginDelivery();

            var context = new RabbitMqReceiveContext(_inputAddress, exchange, routingKey, _consumerTag, deliveryTag, body, redelivered, properties,
                _context, _receiveSettings, _model, _model.ConnectionContext);

            var activity = LogContext.IfEnabled(OperationName.Transport.Receive)?.StartActivity();
            activity.AddReceiveContextHeaders(context);

            try
            {
                if (!_pending.TryAdd(deliveryTag, context))
                    LogContext.Warning?.Log("Duplicate BasicDeliver: {DeliveryTag}", deliveryTag);

                if (_context.ReceiveObservers.Count > 0)
                    await _context.ReceiveObservers.PreReceive(context).ConfigureAwait(false);

                await _context.ReceivePipe.Send(context).ConfigureAwait(false);

                await context.ReceiveCompleted.ConfigureAwait(false);

                _model.BasicAck(deliveryTag, false);

                if (_context.ReceiveObservers.Count > 0)
                    await _context.ReceiveObservers.PostReceive(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (_context.ReceiveObservers.Count > 0)
                    await _context.ReceiveObservers.ReceiveFault(context, ex).ConfigureAwait(false);

                try
                {
                    _model.BasicNack(deliveryTag, false, true);
                }
                catch (Exception ackEx)
                {
                    LogContext.Error?.Log(ackEx, "Message NACK failed: {DeliveryTag}", deliveryTag);
                }
            }
            finally
            {
                activity?.Stop();

                delivery.Dispose();

                _pending.TryRemove(deliveryTag, out _);
                context.Dispose();
            }
        }

        IModel IBasicConsumer.Model => _model.Model;

        public event EventHandler<ConsumerEventArgs> ConsumerCancelled;

        string RabbitMqDeliveryMetrics.ConsumerTag => _consumerTag;

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

            if (_tracker.ActiveDeliveryCount > 0)
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
