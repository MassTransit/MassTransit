namespace MassTransit.AmazonSqsTransport.Contexts
{
    using System;
    using System.Collections.Generic;
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
    using GreenPipes;
    using Pipeline;
    using Topology;
    using Topology.Entities;
    using Transports;
    using Util;


    public class AmazonSqsClientContext :
        ScopePipeContext,
        ClientContext
    {
        readonly IAmazonSimpleNotificationService _amazonSns;
        readonly IAmazonSQS _amazonSqs;
        readonly CancellationToken _cancellationToken;
        readonly ConnectionContext _connectionContext;
        readonly LimitedConcurrencyLevelTaskScheduler _taskScheduler;
        readonly QueueCache _queueCache;
        readonly TopicCache _topicCache;

        public AmazonSqsClientContext(ConnectionContext connectionContext, IAmazonSQS amazonSqs, IAmazonSimpleNotificationService amazonSns,
            CancellationToken cancellationToken)
            : base(connectionContext)
        {
            _connectionContext = connectionContext;
            _amazonSqs = amazonSqs;
            _amazonSns = amazonSns;
            _cancellationToken = cancellationToken;

            _queueCache = new QueueCache(amazonSqs);
            _topicCache = new TopicCache(amazonSns);

            _taskScheduler = new LimitedConcurrencyLevelTaskScheduler(1);
        }

        public Task DisposeAsync(CancellationToken cancellationToken)
        {
            _queueCache.Clear();
            _topicCache.Clear();

            _amazonSqs?.Dispose();
            _amazonSns?.Dispose();

            return TaskUtil.Completed;
        }

        CancellationToken PipeContext.CancellationToken => _cancellationToken;

        ConnectionContext ClientContext.ConnectionContext => _connectionContext;

        public Task<TopicInfo> CreateTopic(Topology.Entities.Topic topic)
        {
            return _topicCache.Get(topic, _cancellationToken);
        }

        public Task<QueueInfo> CreateQueue(Queue queue)
        {
            return _queueCache.Get(queue, _cancellationToken);
        }

        async Task ClientContext.CreateQueueSubscription(Topology.Entities.Topic topic, Queue queue)
        {
            var topicInfo = await _topicCache.Get(topic, _cancellationToken).ConfigureAwait(false);
            var queueInfo = await _queueCache.Get(queue, _cancellationToken).ConfigureAwait(false);

            var subscriptionAttributes = topic.TopicSubscriptionAttributes.Select(x => (x.Key, x.Value.ToString()))
                .Concat(queue.QueueSubscriptionAttributes.Select(x => (x.Key, x.Value.ToString())))
                .ToDictionary(x => x.Item1, x => x.Item2);

            var subscribeRequest = new SubscribeRequest
            {
                TopicArn = topicInfo.Arn,
                Endpoint = queueInfo.Arn,
                Protocol = "sqs",
                Attributes = subscriptionAttributes
            };

            var response = await _amazonSns.SubscribeAsync(subscribeRequest, _cancellationToken).ConfigureAwait(false);

            response.EnsureSuccessfulResponse();

            var sqsQueueArn = queueInfo.Arn;
            var topicArnPattern = topicInfo.Arn.Substring(0, topicInfo.Arn.LastIndexOf(':') + 1) + "*";

            queueInfo.Attributes.TryGetValue(QueueAttributeName.Policy, out var policyValue);
            var policy = string.IsNullOrEmpty(policyValue)
                ? new Policy()
                : Policy.FromJson(policyValue);

            if (!QueueHasTopicPermission(policy, topicArnPattern, sqsQueueArn))
            {
                var statement = new Statement(Statement.StatementEffect.Allow);
                statement.Actions.Add(SQSActionIdentifiers.SendMessage);
                statement.Resources.Add(new Resource(sqsQueueArn));
                statement.Conditions.Add(ConditionFactory.NewSourceArnCondition(topicArnPattern));
                statement.Principals.Add(new Principal("*"));
                policy.Statements.Add(statement);

                var jsonPolicy = policy.ToJson();

                var setAttributes = new Dictionary<string, string> {{QueueAttributeName.Policy, jsonPolicy}};
                var setAttributesResponse = await _amazonSqs.SetQueueAttributesAsync(queueInfo.Url, setAttributes, _cancellationToken).ConfigureAwait(false);

                setAttributesResponse.EnsureSuccessfulResponse();

                queueInfo.Attributes[QueueAttributeName.Policy] = jsonPolicy;
            }
        }

        async Task ClientContext.DeleteTopic(Topology.Entities.Topic topic)
        {
            var topicInfo = await _topicCache.Get(topic, _cancellationToken).ConfigureAwait(false);

            TransportLogMessages.DeleteTopic(topicInfo.Arn);

            var response = await _amazonSns.DeleteTopicAsync(topicInfo.Arn, _cancellationToken).ConfigureAwait(false);

            response.EnsureSuccessfulResponse();

            _topicCache.RemoveByName(topic.EntityName);
        }

        async Task ClientContext.DeleteQueue(Queue queue)
        {
            var queueInfo = await _queueCache.Get(queue, _cancellationToken).ConfigureAwait(false);

            TransportLogMessages.DeleteQueue(queueInfo.Url);

            var response = await _amazonSqs.DeleteQueueAsync(queueInfo.Url, _cancellationToken).ConfigureAwait(false);

            response.EnsureSuccessfulResponse();

            _queueCache.RemoveByName(queue.EntityName);
        }

        async Task ClientContext.BasicConsume(ReceiveSettings receiveSettings, IBasicConsumer consumer)
        {
            var queueInfo = await _queueCache.GetByName(receiveSettings.EntityName).ConfigureAwait(false);

            await Task.Factory.StartNew(async () =>
            {
                while (!CancellationToken.IsCancellationRequested)
                {
                    List<Message> messages = await PollMessages(queueInfo.Url, receiveSettings).ConfigureAwait(false);

                    await Task.WhenAll(messages.Select(consumer.HandleMessage)).ConfigureAwait(false);
                }
            }, CancellationToken, TaskCreationOptions.None, _taskScheduler).ConfigureAwait(false);
        }

        async Task<PublishRequest> ClientContext.CreatePublishRequest(string topicName, byte[] body)
        {
            var topicInfo = await _topicCache.GetByName(topicName).ConfigureAwait(false);

            var message = Encoding.UTF8.GetString(body);

            return new PublishRequest(topicInfo.Arn, message);
        }

        async Task<SendMessageRequest> ClientContext.CreateSendRequest(string queueName, byte[] body)
        {
            var queueInfo = await _queueCache.GetByName(queueName).ConfigureAwait(false);

            var message = Encoding.UTF8.GetString(body);

            return new SendMessageRequest(queueInfo.Url, message);
        }

        async Task ClientContext.Publish(PublishRequest request, CancellationToken cancellationToken)
        {
            var response = await _amazonSns.PublishAsync(request, cancellationToken).ConfigureAwait(false);

            response.EnsureSuccessfulResponse();
        }

        async Task ClientContext.SendMessage(SendMessageRequest request, CancellationToken cancellationToken)
        {
            var response = await _amazonSqs.SendMessageAsync(request, cancellationToken).ConfigureAwait(false);

            response.EnsureSuccessfulResponse();
        }

        async Task ClientContext.DeleteMessage(string queueName, string receiptHandle, CancellationToken cancellationToken)
        {
            var queueInfo = await _queueCache.GetByName(queueName).ConfigureAwait(false);

            var response = await _amazonSqs.DeleteMessageAsync(queueInfo.Url, receiptHandle, cancellationToken).ConfigureAwait(false);

            response.EnsureSuccessfulResponse();
        }

        async Task ClientContext.PurgeQueue(string queueName, CancellationToken cancellationToken)
        {
            var queueInfo = await _queueCache.GetByName(queueName).ConfigureAwait(false);

            var response = await _amazonSqs.PurgeQueueAsync(queueInfo.Url, cancellationToken).ConfigureAwait(false);

            response.EnsureSuccessfulResponse();
        }

        static bool QueueHasTopicPermission(Policy policy, string topicArnPattern, string sqsQueueArn)
        {
            IEnumerable<Condition> conditions = policy.Statements
                .Where(s => s.Resources.Any(r => r.Id.Equals(sqsQueueArn)))
                .SelectMany(s => s.Conditions);

            return conditions.Any(c =>
                string.Equals(c.Type, ConditionFactory.ArnComparisonType.ArnLike.ToString(), StringComparison.OrdinalIgnoreCase) &&
                string.Equals(c.ConditionKey, ConditionFactory.SOURCE_ARN_CONDITION_KEY, StringComparison.OrdinalIgnoreCase) &&
                c.Values.Contains(topicArnPattern));
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
            const int awsMax = 10;

            var remaining = receiveSettings.PrefetchCount % awsMax;
            var receives = Enumerable.Repeat(awsMax, receiveSettings.PrefetchCount / awsMax).ToList();

            if (remaining > 0)
            {
                receives.Add(remaining);
            }

            var responses = await Task.WhenAll(receives.Select(numberOfMessages => ReceiveMessages(receiveSettings, queueUrl, numberOfMessages)))
                .ConfigureAwait(false);

            return responses.SelectMany(r => r.Messages).ToList();
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

            var response = await _amazonSqs.ReceiveMessageAsync(request, CancellationToken).ConfigureAwait(false);

            response.EnsureSuccessfulResponse();

            return response;
        }
    }
}
