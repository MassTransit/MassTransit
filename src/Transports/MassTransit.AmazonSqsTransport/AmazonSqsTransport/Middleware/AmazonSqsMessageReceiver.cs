namespace MassTransit.AmazonSqsTransport.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.SQS;
    using Amazon.SQS.Model;
    using Internals;
    using MassTransit.Middleware;
    using Transports;
    using Util;


    /// <summary>
    /// Receives messages from AmazonSQS, pushing them to the InboundPipe of the service endpoint.
    /// </summary>
    public sealed class AmazonSqsMessageReceiver :
        ConsumerAgent<string>
    {
        readonly ClientContext _client;
        readonly SqsReceiveEndpointContext _context;
        readonly IChannelExecutorPool<Message> _executorPool;
        readonly ReceiveSettings _receiveSettings;

        /// <summary>
        /// The basic consumer receives messages pushed from the broker.
        /// </summary>
        /// <param name="client">The model context for the consumer</param>
        /// <param name="context">The topology</param>
        public AmazonSqsMessageReceiver(ClientContext client, SqsReceiveEndpointContext context)
            : base(context, StringComparer.Ordinal)
        {
            _client = client;
            _context = context;

            _receiveSettings = client.GetPayload<ReceiveSettings>();

            _executorPool = new FifoChannelExecutorPool(_receiveSettings);

            TrySetConsumeTask(Task.Run(() => Consume()));
        }

        protected override async Task ActiveAndActualAgentsCompleted(StopContext context)
        {
            await base.ActiveAndActualAgentsCompleted(context).ConfigureAwait(false);

            await _executorPool.DisposeAsync().ConfigureAwait(false);
        }

        async Task Consume()
        {
            await GetQueueAttributes().ConfigureAwait(false);

            using var algorithm = new RequestRateAlgorithm(new RequestRateAlgorithmOptions
            {
                PrefetchCount = _receiveSettings.PrefetchCount,
                ConcurrentResultLimit = _receiveSettings.ConcurrentMessageLimit,
                RequestResultLimit = 10
            });

            SetReady();

            Task Handle(Message message, CancellationToken cancellationToken)
            {
                var lockContext = new AmazonSqsReceiveLockContext(_context.InputAddress, message, _receiveSettings, _client);

                return _receiveSettings.IsOrdered
                    ? _executorPool.Run(message, () => HandleMessage(message, lockContext), cancellationToken)
                    : HandleMessage(message, lockContext);
            }

            try
            {
                while (!IsStopping)
                    await algorithm.Run(ReceiveMessages, (m, c) => Handle(m, c), Stopping).ConfigureAwait(false);
            }
            catch (OperationCanceledException exception) when (exception.CancellationToken == Stopping)
            {
            }
            catch (Exception exception)
            {
                LogContext.Warning?.Log(exception, "Consume Loop faulted");
            }
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

        async Task HandleMessage(Message message, ReceiveLockContext lockContext)
        {
            if (IsStopping)
                return;

            var redelivered = message.Attributes.TryGetInt("ApproximateReceiveCount", out var receiveCount) && receiveCount > 1;

            var context = new AmazonSqsReceiveContext(message, redelivered, _context, _client, _receiveSettings, _client.ConnectionContext);
            try
            {
                await Dispatch(message.MessageId, context, lockContext).ConfigureAwait(false);
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

        async Task<IEnumerable<Message>> ReceiveMessages(int messageLimit, CancellationToken cancellationToken)
        {
            try
            {
                return await _client.ReceiveMessages(_receiveSettings.EntityName, messageLimit, _receiveSettings.WaitTimeSeconds, cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return [];
            }
        }


        class FifoChannelExecutorPool :
            IChannelExecutorPool<Message>
        {
            readonly IChannelExecutorPool<Message> _keyExecutorPool;

            public FifoChannelExecutorPool(ReceiveSettings receiveSettings)
            {
                IHashGenerator hashGenerator = new Murmur3UnsafeHashGenerator();
                _keyExecutorPool = new PartitionChannelExecutorPool<Message>(MessageGroupIdProvider, hashGenerator,
                    receiveSettings.ConcurrentMessageLimit, receiveSettings.ConcurrentDeliveryLimit);
            }

            public Task Push(Message result, Func<Task> handle, CancellationToken cancellationToken)
            {
                return _keyExecutorPool.Push(result, handle, cancellationToken);
            }

            public Task Run(Message result, Func<Task> method, CancellationToken cancellationToken = default)
            {
                return _keyExecutorPool.Run(result, method, cancellationToken);
            }

            public ValueTask DisposeAsync()
            {
                return _keyExecutorPool.DisposeAsync();
            }

            static byte[] MessageGroupIdProvider(Message message)
            {
                return message.Attributes.TryGetValue(MessageSystemAttributeName.MessageGroupId, out var groupId) && !string.IsNullOrEmpty(groupId)
                    ? Encoding.UTF8.GetBytes(groupId)
                    : [];
            }
        }
    }
}
