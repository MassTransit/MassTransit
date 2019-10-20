namespace MassTransit.AmazonSqsTransport.Contexts
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.Auth.AccessControlPolicy;
    using Amazon.Auth.AccessControlPolicy.ActionIdentifiers;
    using Amazon.SimpleNotificationService;
    using Amazon.SimpleNotificationService.Model;
    using Amazon.SQS;
    using Amazon.SQS.Model;
    using Context;
    using GreenPipes;
    using Pipeline;
    using Topology;
    using Topology.Entities;
    using Util;
    using Topic = Topology.Entities.Topic;


    public class AmazonSqsClientContext :
        ScopePipeContext,
        ClientContext,
        IAsyncDisposable
    {
        readonly ConnectionContext _connectionContext;
        readonly IAmazonSQS _amazonSqs;
        readonly IAmazonSimpleNotificationService _amazonSns;
        readonly CancellationToken _cancellationToken;
        readonly LimitedConcurrencyLevelTaskScheduler _taskScheduler;
        readonly object _lock = new object();
        readonly IDictionary<string, string> _queueUrls;
        readonly IDictionary<string, string> _topicArns;

        public AmazonSqsClientContext(ConnectionContext connectionContext, IAmazonSQS amazonSqs, IAmazonSimpleNotificationService amazonSns,
            CancellationToken cancellationToken)
            : base(connectionContext)
        {
            _connectionContext = connectionContext;
            _amazonSqs = amazonSqs;
            _amazonSns = amazonSns;
            _cancellationToken = cancellationToken;

            _taskScheduler = new LimitedConcurrencyLevelTaskScheduler(1);

            _queueUrls = new Dictionary<string, string>();
            _topicArns = new Dictionary<string, string>();
        }

        public Task DisposeAsync(CancellationToken cancellationToken)
        {
            _amazonSqs?.Dispose();
            _amazonSns?.Dispose();

            return GreenPipes.Util.TaskUtil.Completed;
        }

        CancellationToken PipeContext.CancellationToken => _cancellationToken;

        ConnectionContext ClientContext.ConnectionContext => _connectionContext;

        public async Task<string> CreateTopic(Topic topic)
        {
            lock (_lock)
                if (_topicArns.TryGetValue(topic.EntityName, out var result))
                    return result;

            var request = new CreateTopicRequest(topic.EntityName)
            {
                Attributes = topic.TopicAttributes.ToDictionary(x => x.Key, x => x.Value.ToString()),
                Tags = topic.TopicTags.Select(x => new Tag { Key = x.Key, Value =  x.Value }).ToList()
            };

            var response = await _amazonSns.CreateTopicAsync(request).ConfigureAwait(false);

            await Task.Delay(500).ConfigureAwait(false);

            var topicArn = response.TopicArn;

            lock (_lock)
                _topicArns[topic.EntityName] = topicArn;

            return topicArn;
        }

        public async Task<string> CreateQueue(Queue queue)
        {
            lock (_lock)
                if (_queueUrls.TryGetValue(queue.EntityName, out var result))
                    return result;

            // required to preserve backwards compability
            if (queue.EntityName.EndsWith(".fifo", true, CultureInfo.InvariantCulture) && !queue.QueueAttributes.ContainsKey(QueueAttributeName.FifoQueue))
            {
                LogContext.Warning?.Log("Using '.fifo' suffix without 'FifoQueue' attribute might cause unexpected behavior.");

                queue.QueueAttributes[QueueAttributeName.FifoQueue] = true;
            }

            var request = new CreateQueueRequest(queue.EntityName)
            {
                Attributes = queue.QueueAttributes.ToDictionary(x => x.Key, x => x.Value.ToString()),
                Tags = queue.QueueTags.ToDictionary(x => x.Key, x => x.Value)
            };

            var response = await _amazonSqs.CreateQueueAsync(request).ConfigureAwait(false);

            await Task.Delay(500).ConfigureAwait(false);

            var queueUrl = response.QueueUrl;

            lock (_lock)
                _queueUrls[queue.EntityName] = queueUrl;

            return queueUrl;
        }

        async Task ClientContext.CreateQueueSubscription(Topic topic, Queue queue)
        {
            var results = await Task.WhenAll(CreateTopic(topic), CreateQueue(queue)).ConfigureAwait(false);
            var topicArn = results[0];
            var queueUrl = results[1];

            var queueAttributes = await _amazonSqs.GetAttributesAsync(queueUrl).ConfigureAwait(false);
            var queueArn = queueAttributes[QueueAttributeName.QueueArn];

            var topicSubscriptionAttributes = topic.TopicSubscriptionAttributes;
            var queueSubscriptionAttributes = queue.QueueSubscriptionAttributes;
            var subscriptionAttributes = new Dictionary<string, string>();
            topicSubscriptionAttributes.ToList().ForEach(x => subscriptionAttributes[x.Key] = x.Value.ToString());
            queueSubscriptionAttributes.ToList().ForEach(x => subscriptionAttributes[x.Key] = x.Value.ToString());

            var subscribeRequest = new SubscribeRequest
            {
                TopicArn = topicArn,
                Endpoint = queueArn,
                Protocol = "sqs",
                Attributes = subscriptionAttributes,
            };

            await _amazonSns.SubscribeAsync(subscribeRequest).ConfigureAwait(false);

            var sqsQueueArn = queueAttributes[QueueAttributeName.QueueArn];
            var topicArnPattern = topicArn.Substring(0, topicArn.LastIndexOf(':') + 1) + "*";

            queueAttributes.TryGetValue(QueueAttributeName.Policy, out var policyStr);
            var policy = string.IsNullOrEmpty(policyStr) ? new Policy() : Policy.FromJson(policyStr);

            if (!QueueHasTopicPermission(policy, topicArnPattern, sqsQueueArn))
            {
                var statement = new Statement(Statement.StatementEffect.Allow);
                statement.Actions.Add(SQSActionIdentifiers.SendMessage);
                statement.Resources.Add(new Resource(sqsQueueArn));
                statement.Conditions.Add(ConditionFactory.NewSourceArnCondition(topicArnPattern));
                statement.Principals.Add(new Principal("*"));
                policy.Statements.Add(statement);

                var setAttributes = new Dictionary<string, string> {{QueueAttributeName.Policy, policy.ToJson()}};
                await _amazonSqs.SetAttributesAsync(queueUrl, setAttributes).ConfigureAwait(false);
            }
        }

        static bool QueueHasTopicPermission(Policy policy, string topicArnPattern, string sqsQueueArn)
        {
            var conditions = policy.Statements
                .Where(s => s.Resources.Any(r => r.Id.Equals(sqsQueueArn)))
                .SelectMany(s => s.Conditions);

            return conditions.Any(c =>
                string.Equals(c.Type, ConditionFactory.ArnComparisonType.ArnLike.ToString(), StringComparison.OrdinalIgnoreCase) &&
                string.Equals(c.ConditionKey, ConditionFactory.SOURCE_ARN_CONDITION_KEY, StringComparison.OrdinalIgnoreCase) &&
                c.Values.Contains(topicArnPattern));
        }

        async Task ClientContext.DeleteTopic(Topic topic)
        {
            var topicArn = await CreateTopic(topic).ConfigureAwait(false);
            await _amazonSns.DeleteTopicAsync(topicArn).ConfigureAwait(false);
        }

        async Task ClientContext.DeleteQueue(Queue queue)
        {
            var queueUrl = await CreateQueue(queue).ConfigureAwait(false);
            await _amazonSqs.DeleteQueueAsync(queueUrl).ConfigureAwait(false);
        }

        Task ClientContext.BasicConsume(ReceiveSettings receiveSettings, IBasicConsumer consumer)
        {
            string queueUrl;
            lock (_lock)
            {
                if (!_queueUrls.TryGetValue(receiveSettings.EntityName, out queueUrl))
                    throw new ArgumentException($"The queue was unknown: {receiveSettings.EntityName}", nameof(receiveSettings));
            }

            return Task.Factory.StartNew(async () =>
            {
                while (!CancellationToken.IsCancellationRequested)
                {
                    List<Message> messages = await PollMessages(queueUrl, receiveSettings).ConfigureAwait(false);

                    await Task.WhenAll(messages.Select(consumer.HandleMessage)).ConfigureAwait(false);
                }
            }, CancellationToken, TaskCreationOptions.None, _taskScheduler);
        }

        /// <summary>
        /// SQS can only be polled for 10 messages at a time.
        /// Make multiple poll requests, if necessary, to achieve up to PrefetchCount number of messages
        /// </summary>
        /// <param name="queueUrl">URL for queue to be polled</param>
        /// <param name="receiveSettings"></param>
        /// <returns></returns>
        async Task<List<Message>> PollMessages(string queueUrl, ReceiveSettings receiveSettings)
        {
            var messages = new List<Message>();
            var remainingNumMessagesToPoll = receiveSettings.PrefetchCount;
            while (remainingNumMessagesToPoll > 0)
            {
                var numMessagesToPoll = Math.Min(remainingNumMessagesToPoll, 10);
                var response = await ReceiveMessages(receiveSettings, queueUrl, numMessagesToPoll).ConfigureAwait(false);
                var numMessagesReceived = response.Messages.Count;

                messages.AddRange(response.Messages);

                if (numMessagesReceived == 0)
                {
                    break;
                }

                remainingNumMessagesToPoll -= numMessagesReceived;
            }

            return messages;
        }

        async Task<ReceiveMessageResponse> ReceiveMessages(ReceiveSettings receiveSettings, string queueUrl, int maxNumberOfMessages)
        {
            var request = new ReceiveMessageRequest(queueUrl)
            {
                MaxNumberOfMessages = maxNumberOfMessages,
                WaitTimeSeconds = receiveSettings.WaitTimeSeconds,
                AttributeNames = new List<string> {"All"},
                MessageAttributeNames = new List<string> {"All"}
            };

            return await _amazonSqs.ReceiveMessageAsync(request, CancellationToken).ConfigureAwait(false);
        }

        PublishRequest ClientContext.CreatePublishRequest(string topicName, byte[] body)
        {
            var message = Encoding.UTF8.GetString(body);

            lock (_lock)
                if (_topicArns.TryGetValue(topicName, out var topicArn))
                    return new PublishRequest(topicArn, message);

            throw new ArgumentException($"The topic was unknown: {topicName}", nameof(topicName));
        }

        SendMessageRequest ClientContext.CreateSendRequest(string queueName, byte[] body)
        {
            var message = Encoding.UTF8.GetString(body);

            lock (_lock)
                if (_queueUrls.TryGetValue(queueName, out var queueUrl))
                    return new SendMessageRequest(queueUrl, message);

            throw new ArgumentException($"The queue was unknown: {queueName}", nameof(queueName));
        }

        Task ClientContext.Publish(PublishRequest request, CancellationToken cancellationToken)
        {
            return _amazonSns.PublishAsync(request, cancellationToken);
        }

        Task ClientContext.SendMessage(SendMessageRequest request, CancellationToken cancellationToken)
        {
            return _amazonSqs.SendMessageAsync(request, cancellationToken);
        }

        Task ClientContext.DeleteMessage(string queueName, string receiptHandle, CancellationToken cancellationToken)
        {
            lock (_lock)
                if (_queueUrls.TryGetValue(queueName, out var queueUrl))
                    return _amazonSqs.DeleteMessageAsync(queueUrl, receiptHandle, cancellationToken);

            throw new ArgumentException($"The queue was unknown: {queueName}", nameof(queueName));
        }

        Task ClientContext.PurgeQueue(string queueName, CancellationToken cancellationToken)
        {
            lock (_lock)
                if (_queueUrls.TryGetValue(queueName, out var queueUrl))
                    return _amazonSqs.PurgeQueueAsync(queueUrl, cancellationToken);

            throw new ArgumentException($"The queue was unknown: {queueName}", nameof(queueName));
        }
    }
}
