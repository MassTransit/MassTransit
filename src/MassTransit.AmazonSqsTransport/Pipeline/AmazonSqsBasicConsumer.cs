namespace MassTransit.AmazonSqsTransport.Pipeline
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading.Tasks;
    using Amazon.SQS.Model;
    using Context;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using GreenPipes.Internals.Extensions;
    using Logging;
    using Topology;
    using Transports.Metrics;
    using Util;


    /// <summary>
    /// Receives messages from AmazonSQS, pushing them to the InboundPipe of the service endpoint.
    /// </summary>
    public sealed class AmazonSqsBasicConsumer :
        Supervisor,
        IBasicConsumer,
        DeliveryMetrics
    {
        readonly TaskCompletionSource<bool> _deliveryComplete;
        readonly Uri _inputAddress;
        readonly ClientContext _client;
        readonly ConcurrentDictionary<string, AmazonSqsReceiveContext> _pending;
        readonly ReceiveSettings _receiveSettings;
        readonly SqsReceiveEndpointContext _context;
        readonly IDeliveryTracker _tracker;

        /// <summary>
        /// The basic consumer receives messages pushed from the broker.
        /// </summary>
        /// <param name="client">The model context for the consumer</param>
        /// <param name="inputAddress">The input address for messages received by the consumer</param>
        /// <param name="context">The topology</param>
        public AmazonSqsBasicConsumer(ClientContext client, Uri inputAddress, SqsReceiveEndpointContext context)
        {
            _client = client;
            _inputAddress = inputAddress;
            _context = context;

            _tracker = new DeliveryTracker(HandleDeliveryComplete);

            _receiveSettings = client.GetPayload<ReceiveSettings>();

            _pending = new ConcurrentDictionary<string, AmazonSqsReceiveContext>();

            _deliveryComplete = TaskUtil.GetTask<bool>();

            SetReady();
        }

        public static DateTime? FromUnixTime(string unixTime)
        {
            return long.TryParse(unixTime, out long seconds) ? Epoch.AddSeconds(seconds) : default(DateTime?);
        }

        static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public async Task HandleMessage(Message message)
        {
            if (IsStopping)
            {
                await WaitAndAbandonMessage().ConfigureAwait(false);
                return;
            }

            var redelivered = message.Attributes.TryGetValue("ApproximateReceiveCount", out var receiveCountStr)
                && int.TryParse(receiveCountStr, out var receiveCount) && receiveCount > 1;

            var delivery = _tracker.BeginDelivery();

            var context = new AmazonSqsReceiveContext(_inputAddress, message, redelivered, _context, _receiveSettings, _client, _client.ConnectionContext);

            var activity = LogContext.IfEnabled(OperationName.Transport.Receive)?.StartActivity();
            activity.AddReceiveContextHeaders(context);

            try
            {
                if (!_pending.TryAdd(message.MessageId, context))
                    LogContext.Error?.Log("Duplicate message: {MessageId}", message.MessageId);

                if (_context.ReceiveObservers.Count > 0)
                    await _context.ReceiveObservers.PreReceive(context).ConfigureAwait(false);

                await _context.ReceivePipe.Send(context).ConfigureAwait(false);

                await context.ReceiveCompleted.ConfigureAwait(false);

                await _client.DeleteMessage(_receiveSettings.EntityName, message.ReceiptHandle).ConfigureAwait(false);

                if (_context.ReceiveObservers.Count > 0)
                    await _context.ReceiveObservers.PostReceive(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (_context.ReceiveObservers.Count > 0)
                    await _context.ReceiveObservers.ReceiveFault(context, ex).ConfigureAwait(false);
            }
            finally
            {
                activity?.Stop();

                delivery.Dispose();

                _pending.TryRemove(message.MessageId, out _);

                context.Dispose();
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

            if (_tracker.ActiveDeliveryCount > 0)
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
