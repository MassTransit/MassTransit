namespace MassTransit.AmazonSqsTransport
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.Auth.AccessControlPolicy;
    using Amazon.Auth.AccessControlPolicy.ActionIdentifiers;
    using Amazon.SQS;
    using Amazon.SQS.Model;


    public class QueueInfo :
        IAsyncDisposable
    {
        readonly Lazy<IBatcher<DeleteMessageBatchRequestEntry>> _batchDeleter;
        readonly Lazy<IBatcher<SendMessageBatchRequestEntry>> _batchSender;
        readonly IAmazonSQS _client;
        readonly SemaphoreSlim _updateSemaphore;
        bool _disposed;

        public QueueInfo(string entityName, string url, IDictionary<string, string> attributes, IAmazonSQS client, CancellationToken cancellationToken,
            bool existing)
        {
            _client = client;
            Attributes = attributes;
            Existing = existing;
            EntityName = entityName;
            Url = url;

            Arn = attributes.TryGetValue(QueueAttributeName.QueueArn, out var queueArn)
                ? queueArn
                : throw new ArgumentException($"The queueArn was not found: {url}", nameof(attributes));

            _updateSemaphore = new SemaphoreSlim(1);

            _batchSender = new Lazy<IBatcher<SendMessageBatchRequestEntry>>(() => new SendBatcher(client, url, cancellationToken));
            _batchDeleter = new Lazy<IBatcher<DeleteMessageBatchRequestEntry>>(() => new DeleteBatcher(client, url, cancellationToken));

            SubscriptionArns = new List<string>();
        }

        public string EntityName { get; }
        public string Url { get; }
        public string Arn { get; }
        public IDictionary<string, string> Attributes { get; }
        public IList<string> SubscriptionArns { get; }
        public bool Existing { get; }

        public async ValueTask DisposeAsync()
        {
            if (_disposed)
                return;

            _disposed = true;

            _updateSemaphore.Dispose();

            if (_batchSender.IsValueCreated)
                await _batchSender.Value.DisposeAsync().ConfigureAwait(false);
            if (_batchDeleter.IsValueCreated)
                await _batchDeleter.Value.DisposeAsync().ConfigureAwait(false);
        }

        public Task Send(SendMessageBatchRequestEntry entry, CancellationToken cancellationToken)
        {
            return _batchSender.Value.Execute(entry, cancellationToken);
        }

        public Task Delete(string receiptHandle, CancellationToken cancellationToken)
        {
            var entry = new DeleteMessageBatchRequestEntry("", receiptHandle);

            return _batchDeleter.Value.Execute(entry, cancellationToken);
        }

        public async Task<bool> UpdatePolicy(string sqsQueueArn, string topicArn, CancellationToken cancellationToken)
        {
            await _updateSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                Attributes.TryGetValue(QueueAttributeName.Policy, out var policyValue);
                var policy = string.IsNullOrEmpty(policyValue)
                    ? new Policy()
                    : Policy.FromJson(policyValue);

                if (QueueHasTopicPermission(policy, topicArn, sqsQueueArn))
                    return false;

                var statement = policy.Statements.FirstOrDefault();
                if (statement is null)
                {
                    statement = new Statement(Statement.StatementEffect.Allow);
                    #pragma warning disable 618
                    statement.Actions.Add(SQSActionIdentifiers.SendMessage);
                    #pragma warning restore 618
                    statement.Resources.Add(new Resource(sqsQueueArn));
                    statement.Conditions.Add(ConditionFactory.NewSourceArnCondition(topicArn));
                    statement.Principals.Add(new Principal("Service", "sns.amazonaws.com"));
                    policy.Statements.Add(statement);
                }

                var condition = statement.Conditions.First();
                condition.Values = condition
                    .Values
                    .Append(topicArn)
                    .Distinct()
                    .ToArray();

                var jsonPolicy = policy.ToJson();

                var setAttributes = new Dictionary<string, string> { { QueueAttributeName.Policy, jsonPolicy } };
                var setAttributesResponse = await _client.SetQueueAttributesAsync(Url, setAttributes, cancellationToken).ConfigureAwait(false);

                setAttributesResponse.EnsureSuccessfulResponse();

                Attributes[QueueAttributeName.Policy] = jsonPolicy;

                return true;
            }
            finally
            {
                _updateSemaphore.Release();
            }
        }

        static bool QueueHasTopicPermission(Policy policy, string topicArn, string sqsQueueArn)
        {
            var topicArnPattern = topicArn.Substring(0, topicArn.LastIndexOf(':') + 1) + "*";

            IEnumerable<Condition> conditions = policy.Statements
                .Where(s => s.Resources.Any(r => r.Id.Equals(sqsQueueArn)))
                .SelectMany(s => s.Conditions);

            return conditions.Any(c =>
                string.Equals(c.Type, ConditionFactory.ArnComparisonType.ArnLike.ToString(), StringComparison.OrdinalIgnoreCase) &&
                string.Equals(c.ConditionKey, ConditionFactory.SOURCE_ARN_CONDITION_KEY, StringComparison.OrdinalIgnoreCase) &&
                c.Values.Any(v => v == topicArnPattern || v == topicArn));
        }
    }
}
