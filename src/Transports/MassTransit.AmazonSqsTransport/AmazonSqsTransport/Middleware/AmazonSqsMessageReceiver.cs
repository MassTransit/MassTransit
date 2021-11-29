namespace MassTransit.AmazonSqsTransport.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Amazon.SQS;
    using Amazon.SQS.Model;
    using Internals;
    using MassTransit.Middleware;
    using Topology;
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

            SetReady();

            try
            {
                await PollMessages(executor).ConfigureAwait(false);
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

        async Task HandleMessage(Message message)
        {
            if (IsStopping)
                return;

            var redelivered = message.Attributes.TryGetInt("ApproximateReceiveCount", out var receiveCount) && receiveCount > 1;

            var context = new AmazonSqsReceiveContext(message, redelivered, _context, _client, _receiveSettings, _client.ConnectionContext);
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
        }

        async Task PollMessages(ChannelExecutor executor)
        {
            var maxReceiveCount = (_receiveSettings.PrefetchCount + 9) / 10;
            var receiveCount = 1;
            var messageLimit = Math.Min(_receiveSettings.PrefetchCount, 10);

            while (!IsStopping)
            {
                var received = 0;

                if (_receiveSettings.IsOrdered)
                {
                    Task<IList<Message>>[] receiveTasks = Enumerable.Repeat(0, receiveCount).Select(_ => ReceiveOrderedMessages(messageLimit)).ToArray();
                    IList<Message>[] messages = await Task.WhenAll(receiveTasks).ConfigureAwait(false);

                    received = await HandleOrderedMessages(executor, messages.SelectMany(x => x)).ConfigureAwait(false);
                }
                else
                {
                    Task<int>[] receiveTasks = Enumerable.Repeat(0, receiveCount).Select(_ => ReceiveAndHandleMessages(messageLimit, executor)).ToArray();
                    var counts = await Task.WhenAll(receiveTasks).ConfigureAwait(false);

                    received = counts.Sum();
                }

                if (received == receiveCount * 10) // ramp up receivers when busy
                    receiveCount = Math.Min(maxReceiveCount, receiveCount + (maxReceiveCount - receiveCount) / 2);
                else if (received / 10 < receiveCount - 1) // dial it back when not so busy
                    receiveCount = Math.Max(1, (received + 9) / 10);
            }
        }

        async Task<IList<Message>> ReceiveOrderedMessages(int messageLimit)
        {
            try
            {
                return await _client.ReceiveMessages(_receiveSettings.EntityName, messageLimit, _receiveSettings.WaitTimeSeconds, Stopping)
                    .ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return Array.Empty<Message>();
            }
        }

        async Task<int> HandleOrderedMessages(ChannelExecutor executor, IEnumerable<Message> messages)
        {
            IEnumerable<IGrouping<string, Message>> messageGroups = messages
                .GroupBy(x => x.Attributes.TryGetValue(MessageSystemAttributeName.MessageGroupId, out var groupId)
                    ? groupId
                    : "")
                .ToList();

            List<Task> messageGroupTasks = messageGroups.Select(x => HandleOrderedMessageGroup(executor, x)).ToList();

            await Task.WhenAll(messageGroupTasks).ConfigureAwait(false);

            return messageGroups.Sum(x => x.Count());
        }

        async Task HandleOrderedMessageGroup(ChannelExecutor executor, IEnumerable<Message> messages)
        {
            foreach (var message in messages.OrderBy(x => x.Attributes.TryGetValue("SequenceNumber", out var sequenceNumber) ? sequenceNumber : "",
                         SequenceNumberComparer.Instance))
                await executor.Run(() => HandleMessage(message), Stopping).ConfigureAwait(false);
        }

        async Task<int> ReceiveAndHandleMessages(int messageLimit, ChannelExecutor executor)
        {
            try
            {
                IList<Message> messages = await _client.ReceiveMessages(_receiveSettings.EntityName, messageLimit, _receiveSettings.WaitTimeSeconds, Stopping)
                    .ConfigureAwait(false);

                foreach (var message in messages)
                    await executor.Push(() => HandleMessage(message), Stopping).ConfigureAwait(false);

                return messages.Count;
            }
            catch (OperationCanceledException)
            {
                return 0;
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
