namespace MassTransit.AmazonSqsTransport
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.Auth.AccessControlPolicy;
    using Amazon.Auth.AccessControlPolicy.ActionIdentifiers;
    using Amazon.SimpleNotificationService;
    using Amazon.SimpleNotificationService.Model;
    using Amazon.SQS;
    using Amazon.SQS.Model;
    using MassTransit.Middleware;
    using Topology;
    using Transports;


    public class AmazonSqsClientContext :
        ScopePipeContext,
        ClientContext,
        IAsyncDisposable
    {
        readonly CancellationToken _cancellationToken;
        readonly QueueCache _queueCache;
        readonly IAmazonSimpleNotificationService _snsClient;
        readonly IAmazonSQS _sqsClient;
        readonly TopicCache _topicCache;

        public AmazonSqsClientContext(ConnectionContext connectionContext, IAmazonSQS sqsClient, IAmazonSimpleNotificationService snsClient,
            CancellationToken cancellationToken)
            : base(connectionContext)
        {
            ConnectionContext = connectionContext;

            _sqsClient = sqsClient;
            _snsClient = snsClient;
            _cancellationToken = cancellationToken;

            _queueCache = new QueueCache(sqsClient, cancellationToken);
            _topicCache = new TopicCache(snsClient, cancellationToken);
        }

        public override CancellationToken CancellationToken => _cancellationToken;

        public ConnectionContext ConnectionContext { get; }

        public Task<TopicInfo> CreateTopic(Topology.Topic topic)
        {
            return _topicCache.Get(topic);
        }

        public Task<QueueInfo> CreateQueue(Queue queue)
        {
            return _queueCache.Get(queue);
        }

        public async Task CreateQueueSubscription(Topology.Topic topic, Queue queue)
        {
            var topicInfo = await _topicCache.Get(topic).ConfigureAwait(false);
            var queueInfo = await _queueCache.Get(queue).ConfigureAwait(false);

            Dictionary<string, string> subscriptionAttributes = topic.TopicSubscriptionAttributes.Select(x => (x.Key, x.Value.ToString()))
                .Concat(queue.QueueSubscriptionAttributes.Select(x => (x.Key, x.Value.ToString())))
                .ToDictionary(x => x.Item1, x => x.Item2);

            var subscribeRequest = new SubscribeRequest
            {
                TopicArn = topicInfo.Arn,
                Endpoint = queueInfo.Arn,
                Protocol = "sqs",
                Attributes = subscriptionAttributes
            };

            var response = await _snsClient.SubscribeAsync(subscribeRequest, CancellationToken).ConfigureAwait(false);

            response.EnsureSuccessfulResponse();

            queueInfo.SubscriptionArns.Add(response.SubscriptionArn);

            var sqsQueueArn = queueInfo.Arn;
            var topicArnPattern = topicInfo.Arn.Substring(0, topicInfo.Arn.LastIndexOf(':') + 1) + "*";

            queueInfo.Attributes.TryGetValue(QueueAttributeName.Policy, out var policyValue);
            var policy = string.IsNullOrEmpty(policyValue)
                ? new Policy()
                : Policy.FromJson(policyValue);

            if (!QueueHasTopicPermission(policy, topicArnPattern, sqsQueueArn))
            {
                var statement = new Statement(Statement.StatementEffect.Allow);
            #pragma warning disable 618
                statement.Actions.Add(SQSActionIdentifiers.SendMessage);
            #pragma warning restore 618
                statement.Resources.Add(new Resource(sqsQueueArn));
                statement.Conditions.Add(ConditionFactory.NewSourceArnCondition(topicArnPattern));
                statement.Principals.Add(new Principal("*"));
                policy.Statements.Add(statement);

                var jsonPolicy = policy.ToJson();

                var setAttributes = new Dictionary<string, string> { { QueueAttributeName.Policy, jsonPolicy } };
                var setAttributesResponse = await _sqsClient.SetQueueAttributesAsync(queueInfo.Url, setAttributes, CancellationToken).ConfigureAwait(false);

                setAttributesResponse.EnsureSuccessfulResponse();

                queueInfo.Attributes[QueueAttributeName.Policy] = jsonPolicy;
            }
        }

        public async Task DeleteTopic(Topology.Topic topic)
        {
            var topicInfo = await _topicCache.Get(topic).ConfigureAwait(false);

            TransportLogMessages.DeleteTopic(topicInfo.Arn);

            var response = await _snsClient.DeleteTopicAsync(topicInfo.Arn, CancellationToken.None).ConfigureAwait(false);

            response.EnsureSuccessfulResponse();

            _topicCache.RemoveByName(topic.EntityName);
        }

        public async Task DeleteQueue(Queue queue)
        {
            var queueInfo = await _queueCache.Get(queue).ConfigureAwait(false);

            TransportLogMessages.DeleteQueue(queueInfo.Url);

            foreach (var subscriptionArn in queueInfo.SubscriptionArns)
            {
                TransportLogMessages.DeleteSubscription(queueInfo.Url, subscriptionArn);

                await DeleteQueueSubscription(subscriptionArn).ConfigureAwait(false);
            }

            var response = await _sqsClient.DeleteQueueAsync(queueInfo.Url, CancellationToken.None).ConfigureAwait(false);

            response.EnsureSuccessfulResponse();

            _queueCache.RemoveByName(queue.EntityName);
        }

        public async Task<PublishRequest> CreatePublishRequest(string topicName, string body)
        {
            var topicInfo = await _topicCache.GetByName(topicName).ConfigureAwait(false);

            return new PublishRequest(topicInfo.Arn, body);
        }

        public async Task Publish(PublishRequest request, CancellationToken cancellationToken)
        {
            var response = await _snsClient.PublishAsync(request, cancellationToken).ConfigureAwait(false);

            response.EnsureSuccessfulResponse();
        }

        public async Task SendMessage(string queueName, SendMessageBatchRequestEntry request, CancellationToken cancellationToken)
        {
            var queueInfo = await _queueCache.GetByName(queueName).ConfigureAwait(false);

            await queueInfo.Send(request, cancellationToken).ConfigureAwait(false);
        }

        public async Task DeleteMessage(string queueName, string receiptHandle, CancellationToken cancellationToken)
        {
            var queueInfo = await _queueCache.GetByName(queueName).ConfigureAwait(false);

            await queueInfo.Delete(receiptHandle, cancellationToken).ConfigureAwait(false);
        }

        public async Task PurgeQueue(string queueName, CancellationToken cancellationToken)
        {
            var queueInfo = await _queueCache.GetByName(queueName).ConfigureAwait(false);

            var response = await _sqsClient.PurgeQueueAsync(queueInfo.Url, cancellationToken).ConfigureAwait(false);

            response.EnsureSuccessfulResponse();
        }

        public async Task<IList<Message>> ReceiveMessages(string queueName, int messageLimit, int waitTime, CancellationToken cancellationToken)
        {
            var queueInfo = await _queueCache.GetByName(queueName).ConfigureAwait(false);

            var request = new ReceiveMessageRequest(queueInfo.Url)
            {
                MaxNumberOfMessages = messageLimit,
                WaitTimeSeconds = waitTime,
                AttributeNames = new List<string> { "All" },
                MessageAttributeNames = new List<string> { "All" }
            };

            var response = await _sqsClient.ReceiveMessageAsync(request, cancellationToken).ConfigureAwait(false);

            response.EnsureSuccessfulResponse();

            return response.Messages;
        }

        public Task<QueueInfo> GetQueueInfo(string queueName)
        {
            return _queueCache.GetByName(queueName);
        }

        public async Task ChangeMessageVisibility(string queueUrl, string receiptHandle, int seconds)
        {
            var response = await _sqsClient.ChangeMessageVisibilityAsync(new ChangeMessageVisibilityRequest
            {
                QueueUrl = queueUrl,
                ReceiptHandle = receiptHandle,
                VisibilityTimeout = seconds
            }, CancellationToken).ConfigureAwait(false);

            response.EnsureSuccessfulResponse();
        }

        public async ValueTask DisposeAsync()
        {
            await _queueCache.DisposeAsync().ConfigureAwait(false);

            _topicCache.Clear();
        }

        async Task DeleteQueueSubscription(string subscriptionArn)
        {
            var unsubscribeRequest = new UnsubscribeRequest { SubscriptionArn = subscriptionArn };

            var response = await _snsClient.UnsubscribeAsync(unsubscribeRequest, CancellationToken.None).ConfigureAwait(false);

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
    }
}
