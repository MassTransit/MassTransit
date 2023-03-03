namespace MassTransit.AmazonSqsTransport.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.SQS;
    using Amazon.SQS.Model;
    using Internals;
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

            TrySetConsumeTask(Task.Run(() => Consume()));
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

            try
            {
                while (!IsStopping)
                {
                    if (_receiveSettings.IsOrdered)
                    {
                        await algorithm.Run(ReceiveMessages, (m, _) => HandleMessage(m), GroupMessages, OrderMessages, Stopping)
                            .ConfigureAwait(false);
                    }
                    else
                        await algorithm.Run(ReceiveMessages, (m, _) => HandleMessage(m), Stopping).ConfigureAwait(false);
                }
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

        async Task HandleMessage(Message message)
        {
            if (IsStopping)
                return;

            var redelivered = message.Attributes.TryGetInt("ApproximateReceiveCount", out var receiveCount) && receiveCount > 1;

            var context = new AmazonSqsReceiveContext(message, redelivered, _context, _client, _receiveSettings, _client.ConnectionContext);
            try
            {
                await Dispatch(message.MessageId, context, ctx => new AmazonSqsReceiveLockContext(ctx, message, _receiveSettings, _client))
                    .ConfigureAwait(false);
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

        static IEnumerable<IGrouping<string, Message>> GroupMessages(IEnumerable<Message> messages)
        {
            return messages.GroupBy(x => x.Attributes.TryGetValue(MessageSystemAttributeName.MessageGroupId, out var groupId) ? groupId : "");
        }

        static IEnumerable<Message> OrderMessages(IEnumerable<Message> messages)
        {
            return messages.OrderBy(x => x.Attributes.TryGetValue("SequenceNumber", out var sequenceNumber) ? sequenceNumber : "",
                SequenceNumberComparer.Instance);
        }

        async Task<IEnumerable<Message>> ReceiveMessages(int messageLimit, CancellationToken cancellationToken)
        {
            try
            {
                return await _client
                    .ReceiveMessages(_receiveSettings.EntityName, messageLimit, _receiveSettings.WaitTimeSeconds, cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return Array.Empty<Message>();
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
