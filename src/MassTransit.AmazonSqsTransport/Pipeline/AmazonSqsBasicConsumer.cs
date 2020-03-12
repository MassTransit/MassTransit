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
    using Topology;
    using Transports;
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
        readonly ClientContext _client;
        readonly ConcurrentDictionary<string, AmazonSqsReceiveContext> _pending;
        readonly ReceiveSettings _receiveSettings;
        readonly SqsReceiveEndpointContext _context;
        readonly IReceivePipeDispatcher _dispatcher;

        /// <summary>
        /// The basic consumer receives messages pushed from the broker.
        /// </summary>
        /// <param name="client">The model context for the consumer</param>
        /// <param name="context">The topology</param>
        public AmazonSqsBasicConsumer(ClientContext client, SqsReceiveEndpointContext context)
        {
            _client = client;
            _context = context;

            _receiveSettings = client.GetPayload<ReceiveSettings>();

            _pending = new ConcurrentDictionary<string, AmazonSqsReceiveContext>();

            _deliveryComplete = TaskUtil.GetTask<bool>();

            _dispatcher = context.CreateReceivePipeDispatcher();
            _dispatcher.ZeroActivity += HandleDeliveryComplete;

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

            var context = new AmazonSqsReceiveContext(message, redelivered, _context, _client, _receiveSettings, _client.ConnectionContext);
            if (!_pending.TryAdd(message.MessageId, context))
                LogContext.Error?.Log("Duplicate message: {MessageId}", message.MessageId);

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
                _pending.TryRemove(message.MessageId, out _);

                context.Dispose();
            }
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
        }
    }
}
