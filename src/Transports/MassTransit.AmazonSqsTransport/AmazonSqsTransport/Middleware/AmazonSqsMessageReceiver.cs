namespace MassTransit.AmazonSqsTransport.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.SQS;
    using Internals;
    using MassTransit.Middleware;
    using Transports;
    using Util;


    /// <summary>
    /// Receives messages from AmazonSQS, pushing them to the InboundPipe of the service endpoint.
    /// </summary>
    public sealed class AmazonSqsMessageReceiver :
        Agent,
        DeliveryMetrics
    {
        readonly ClientContext _client;
        readonly SqsReceiveEndpointContext _context;
        readonly TaskCompletionSource<bool> _deliveryComplete;
        readonly IReceivePipeDispatcher _dispatcher;
        readonly ReceiveSettings _receiveSettings;

        /// <summary>
        /// The basic consumer receives messages pushed from the broker.
        /// </summary>
        /// <param name="client">The model context for the consumer</param>
        /// <param name="context">The topology</param>
        public AmazonSqsMessageReceiver(ClientContext client, SqsReceiveEndpointContext context)
        {
            _client = client;
            _context = context;

            _receiveSettings = client.GetPayload<ReceiveSettings>();

            _deliveryComplete = TaskUtil.GetTask<bool>();

            _dispatcher = context.CreateReceivePipeDispatcher();
            _dispatcher.ZeroActivity += HandleDeliveryComplete;

            var task = Task.Run(Consume);
            SetCompleted(task);
        }

        long DeliveryMetrics.DeliveryCount => _dispatcher.DispatchCount;
        int DeliveryMetrics.ConcurrentDeliveryCount => _dispatcher.MaxConcurrentDispatchCount;

        async Task Consume()
        {
            var executor = new ChannelExecutor(_receiveSettings.PrefetchCount, _receiveSettings.ConcurrentMessageLimit);

            await GetQueueAttributes().ConfigureAwait(false);

            using var algorithm = new RequestRateAlgorithm(new RequestRateAlgorithmOptions()
            {
                PrefetchCount = _receiveSettings.PrefetchCount,
                RequestResultLimit = 10
            });

            SetReady();

            try
            {
                while (!IsStopping)
                {
                    if (_receiveSettings.IsOrdered)
                    {
                        await algorithm.Run(ReceiveMessages, (c, t) => executor.Push(() => HandleMessage(c), t), GroupMessages, OrderMessages, Stopping)
                            .ConfigureAwait(false);
                    }
                    else
                    {
                        await algorithm.Run(ReceiveMessages, (c, t) => executor.Push(() => HandleMessage(c), t), Stopping).ConfigureAwait(false);
                    }
                }
            }
            catch (OperationCanceledException exception) when (exception.CancellationToken == Stopping)
            {
            }
            catch (Exception exception)
            {
                LogContext.Error?.Log(exception, "Consume Loop faulted");
                throw;
            }
            finally
            {
                await executor.DisposeAsync().ConfigureAwait(false);
            }
        }

        protected override async Task StopAgent(StopContext context)
        {
            LogContext.Debug?.Log("Stopping consumer: {InputAddress}", _context.InputAddress);

            SetCompleted(ActiveAndActualAgentsCompleted(context));

            await Completed.ConfigureAwait(false);
        }

        async Task GetQueueAttributes()
        {
            var queueInfo = await _client.GetQueueInfo(_receiveSettings.EntityName).ConfigureAwait(false);

            _receiveSettings.QueueUrl = queueInfo.Url;

            if (queueInfo.Attributes.TryGetValue(QueueAttributeName.VisibilityTimeout, out var value)
                && int.TryParse(value, out var visibilityTimeout)
                && visibilityTimeout != _receiveSettings.VisibilityTimeout)
            {
                LogContext.Debug?.Log("Using queue visibility timeout of {VisibilityTimeout}", TimeSpan.FromSeconds(visibilityTimeout).ToFriendlyString());

                _receiveSettings.VisibilityTimeout = visibilityTimeout;
            }
        }

        async Task HandleMessage(AmazonSqsReceiveContext context)
        {
            try
            {
                if (IsStopping)
                    return;

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
        }

        static IEnumerable<IGrouping<string, AmazonSqsReceiveContext>> GroupMessages(IEnumerable<AmazonSqsReceiveContext> messages)
        {
            return messages.GroupBy(x => x.TransportMessage.Attributes.TryGetValue(MessageSystemAttributeName.MessageGroupId, out var groupId) ? groupId : "");
        }

        static IEnumerable<AmazonSqsReceiveContext> OrderMessages(IEnumerable<AmazonSqsReceiveContext> messages)
        {
            return messages.OrderBy(x => x.TransportMessage.Attributes.TryGetValue("SequenceNumber", out var sequenceNumber) ? sequenceNumber : "",
                SequenceNumberComparer.Instance);
        }

        async Task<IEnumerable<AmazonSqsReceiveContext>> ReceiveMessages(int messageLimit, CancellationToken cancellationToken)
        {
            try
            {
                var messages = await _client.ReceiveMessages(_receiveSettings.EntityName, messageLimit, _receiveSettings.WaitTimeSeconds, cancellationToken)
                    .ConfigureAwait(false);

                var receiveTime = DateTime.UtcNow;

                return messages.Select(message => new AmazonSqsReceiveContext(
                    message,
                    redelivered: message.Attributes.TryGetInt("ApproximateReceiveCount", out var receiveCount) && receiveCount > 1,
                    receiveTime,
                    _context,
                    _client,
                    _receiveSettings,
                    _client.ConnectionContext)
                );
            }
            catch (OperationCanceledException)
            {
                return Array.Empty<AmazonSqsReceiveContext>();
            }
        }

        Task HandleDeliveryComplete()
        {
            if (IsStopping)
                _deliveryComplete.TrySetResult(true);

            return Task.CompletedTask;
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
        }


        class SequenceNumberComparer :
            IComparer<string>
        {
            public static readonly SequenceNumberComparer Instance = new SequenceNumberComparer();

            public int Compare(string x, string y)
            {
                if (string.IsNullOrWhiteSpace(x))
                    throw new ArgumentNullException(nameof(x));

                if (string.IsNullOrWhiteSpace(y))
                    throw new ArgumentNullException(nameof(y));

                if (x.Length != y.Length)
                    return x.Length > y.Length ? 1 : -1;


                return string.Compare(x, y, StringComparison.Ordinal);
            }
        }
    }
}
