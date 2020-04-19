namespace MassTransit.Azure.ServiceBus.Core.Contexts
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using Internals.Extensions;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.Core;
    using Microsoft.Azure.ServiceBus.Management;
    using Transports;


    public class ServiceBusConnectionContext :
        BasePipeContext,
        ConnectionContext,
        IAsyncDisposable
    {
        readonly ServiceBusConnection _connection;
        readonly ManagementClient _managementClient;
        readonly RetryPolicy _retryPolicy;
        readonly TimeSpan _operationTimeout;

        public ServiceBusConnectionContext(ServiceBusConnection connection, ManagementClient managementClient, RetryPolicy retryPolicy,
            TimeSpan operationTimeout, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _connection = connection;
            _managementClient = managementClient;
            _retryPolicy = retryPolicy;
            _operationTimeout = operationTimeout;
        }

        public Uri Endpoint => _connection.Endpoint;

        public async Task DisposeAsync(CancellationToken cancellationToken)
        {
            var address = _connection.Endpoint.ToString();

            TransportLogMessages.DisconnectHost(address);

            await _managementClient.CloseAsync().ConfigureAwait(false);

            await _connection.CloseAsync().ConfigureAwait(false);

            TransportLogMessages.DisconnectedHost(address);
        }

        public IQueueClient CreateQueueClient(string entityPath)
        {
            return new QueueClient(_connection, entityPath, ReceiveMode.PeekLock, _retryPolicy);
        }

        public ISubscriptionClient CreateSubscriptionClient(string topicPath, string subscriptionName)
        {
            return new SubscriptionClient(_connection, topicPath, subscriptionName, ReceiveMode.PeekLock, _retryPolicy);
        }

        public IMessageSender CreateMessageSender(string entityPath)
        {
            return new MessageSender(_connection, entityPath, _retryPolicy);
        }

        public async Task<QueueDescription> CreateQueue(QueueDescription queueDescription)
        {
            var queueExists = await QueueExistsAsync(queueDescription.Path).ConfigureAwait(false);
            if (queueExists)
            {
                queueDescription = await GetQueueAsync(queueDescription.Path).ConfigureAwait(false);
            }
            else
            {
                try
                {
                    TransportLogMessages.CreateQueue(queueDescription.Path);

                    queueDescription = await CreateQueueAsync(queueDescription).ConfigureAwait(false);
                }
                catch (MessagingEntityAlreadyExistsException)
                {
                    queueDescription = await GetQueueAsync(queueDescription.Path).ConfigureAwait(false);
                }
            }

            LogContext.Debug?.Log("Queue: {Queue} ({Attributes})", queueDescription.Path,
                string.Join(", ",
                    new[]
                    {
                        queueDescription.RequiresDuplicateDetection ? "dupe detect" : "",
                        queueDescription.EnableDeadLetteringOnMessageExpiration ? "dead letter" : "",
                        queueDescription.RequiresSession ? "session" : "",
                        queueDescription.AutoDeleteOnIdle != Defaults.AutoDeleteOnIdle
                            ? $"auto-delete: {queueDescription.AutoDeleteOnIdle.ToFriendlyString()}"
                            : ""
                    }.Where(x => !string.IsNullOrWhiteSpace(x))));

            return queueDescription;
        }

        public async Task<TopicDescription> CreateTopic(TopicDescription topicDescription)
        {
            var topicExists = await TopicExistsAsync(topicDescription.Path).ConfigureAwait(false);
            if (topicExists)
            {
                topicDescription = await GetTopicAsync(topicDescription.Path).ConfigureAwait(false);
            }
            else
            {
                try
                {
                    TransportLogMessages.CreateTopic(topicDescription.Path);

                    topicDescription = await CreateTopicAsync(topicDescription).ConfigureAwait(false);
                }
                catch (MessagingEntityAlreadyExistsException)
                {
                    await GetTopicAsync(topicDescription.Path).ConfigureAwait(false);
                }
            }

            LogContext.Debug?.Log("Topic: {Topic} ({Attributes})", topicDescription.Path,
                string.Join(", ",
                    new[]
                    {
                        topicDescription.RequiresDuplicateDetection ? "dupe detect" : "",
                        topicDescription.EnablePartitioning ? "partitioned" : "",
                        topicDescription.SupportOrdering ? "ordered" : "",
                    }.Where(x => !string.IsNullOrWhiteSpace(x))));

            return topicDescription;
        }

        public async Task<SubscriptionDescription> CreateTopicSubscription(SubscriptionDescription description, RuleDescription rule, Filter filter)
        {
            var create = true;
            SubscriptionDescription subscriptionDescription = null;
            try
            {
                subscriptionDescription = await GetSubscriptionAsync(description.TopicPath, description.SubscriptionName).ConfigureAwait(false);

                string NormalizeForwardTo(string forwardTo)
                {
                    if (string.IsNullOrEmpty(forwardTo))
                        return string.Empty;

                    return forwardTo.Replace(Endpoint.ToString(), string.Empty).Trim('/');
                }

                var targetForwardTo = NormalizeForwardTo(description.ForwardTo);
                var currentForwardTo = NormalizeForwardTo(subscriptionDescription.ForwardTo);

                if (!targetForwardTo.Equals(currentForwardTo))
                {
                    LogContext.Debug?.Log("Updating subscription: {Subscription} ({Topic} -> {ForwardTo})", subscriptionDescription.SubscriptionName,
                        subscriptionDescription.TopicPath, subscriptionDescription.ForwardTo);

                    await UpdateSubscriptionAsync(description).ConfigureAwait(false);
                }

                if (rule != null)
                {
                    RuleDescription ruleDescription = await GetRuleAsync(description.TopicPath, description.SubscriptionName, rule.Name)
                        .ConfigureAwait(false);
                    if (rule.Name == ruleDescription.Name && (rule.Filter != ruleDescription.Filter || rule.Action != ruleDescription.Action))
                    {
                        LogContext.Debug?.Log("Updating subscription Rule: {Rule} ({DescriptionFilter} -> {Filter})", rule.Name,
                            ruleDescription.Filter.ToString(), rule.Filter.ToString());

                        await UpdateRuleAsync(description.TopicPath, description.SubscriptionName, rule).ConfigureAwait(false);
                    }
                }

                create = false;
            }
            catch (MessagingEntityNotFoundException)
            {
            }

            if (create)
            {
                var created = false;
                try
                {
                    LogContext.Debug?.Log("Creating subscription {Subscription} {Topic} -> {ForwardTo}", description.SubscriptionName, description.TopicPath,
                        description.ForwardTo);

                    subscriptionDescription = rule != null
                        ? await CreateSubscriptionAsync(description, rule).ConfigureAwait(false)
                        : filter != null
                            ? await CreateSubscriptionAsync(description, filter).ConfigureAwait(false)
                            : await CreateSubscriptionAsync(description).ConfigureAwait(false);

                    created = true;
                }
                catch (MessagingEntityAlreadyExistsException)
                {
                }

                if (!created)
                    subscriptionDescription = await GetSubscriptionAsync(description.TopicPath, description.SubscriptionName)
                        .ConfigureAwait(false);
            }

            LogContext.Debug?.Log("Subscription {Subscription} ({Topic} -> {ForwardTo})", subscriptionDescription.SubscriptionName,
                subscriptionDescription.TopicPath, subscriptionDescription.ForwardTo);

            return subscriptionDescription;
        }

        public async Task DeleteTopicSubscription(SubscriptionDescription description)
        {
            try
            {
                await DeleteSubscriptionAsync(description.TopicPath, description.SubscriptionName).ConfigureAwait(false);
            }
            catch (MessagingEntityNotFoundException)
            {
            }

            LogContext.Debug?.Log("Subscription Deleted: {Subscription} ({Topic} -> {ForwardTo})", description.SubscriptionName, description.TopicPath,
                description.ForwardTo);
        }

        Task<QueueDescription> GetQueueAsync(string path)
        {
            return RunOperation(() => _managementClient.GetQueueAsync(path));
        }

        Task<bool> QueueExistsAsync(string path)
        {
            return RunOperation(() => _managementClient.QueueExistsAsync(path));
        }

        Task<QueueDescription> CreateQueueAsync(QueueDescription queueDescription)
        {
            return RunOperation(() => _managementClient.CreateQueueAsync(queueDescription));
        }

        Task<TopicDescription> GetTopicAsync(string path)
        {
            return RunOperation(() => _managementClient.GetTopicAsync(path));
        }

        Task<bool> TopicExistsAsync(string path)
        {
            return RunOperation(() => _managementClient.TopicExistsAsync(path));
        }

        Task<TopicDescription> CreateTopicAsync(TopicDescription topicDescription)
        {
            return RunOperation(() => _managementClient.CreateTopicAsync(topicDescription));
        }

        Task<SubscriptionDescription> GetSubscriptionAsync(string topicPath, string subscriptionName)
        {
            return RunOperation(() => _managementClient.GetSubscriptionAsync(topicPath, subscriptionName));
        }

        Task DeleteSubscriptionAsync(string topicPath, string subscriptionName)
        {
            return RunOperation(() => _managementClient.DeleteSubscriptionAsync(topicPath, subscriptionName));
        }

        Task<SubscriptionDescription> UpdateSubscriptionAsync(SubscriptionDescription description)
        {
            return RunOperation(() => _managementClient.UpdateSubscriptionAsync(description));
        }

        Task<RuleDescription> GetRuleAsync(string topicPath, string subscriptionName, string ruleName)
        {
            return RunOperation(() => _managementClient.GetRuleAsync(topicPath, subscriptionName, ruleName));
        }

        Task<RuleDescription> UpdateRuleAsync(string topicPath, string subscriptionName, RuleDescription ruleDescription)
        {
            return RunOperation(() => _managementClient.UpdateRuleAsync(topicPath, subscriptionName, ruleDescription));
        }

        Task<SubscriptionDescription> CreateSubscriptionAsync(SubscriptionDescription description, RuleDescription rule)
        {
            return RunOperation(() => _managementClient.CreateSubscriptionAsync(description, rule));
        }

        Task<SubscriptionDescription> CreateSubscriptionAsync(SubscriptionDescription description, Filter filter)
        {
            var ruleDescription = new RuleDescription(NewId.NextGuid().ToString(), filter);
            return RunOperation(() => _managementClient.CreateSubscriptionAsync(description, ruleDescription));
        }

        Task<SubscriptionDescription> CreateSubscriptionAsync(SubscriptionDescription description)
        {
            return RunOperation(() => _managementClient.CreateSubscriptionAsync(description));
        }

        async Task<T> RunOperation<T>(Func<Task<T>> operation)
        {
            T result = default;

            await _retryPolicy.RunOperation(async () => result = await operation().ConfigureAwait(false), _operationTimeout).ConfigureAwait(false);

            return result;
        }

        Task RunOperation(Func<Task> operation)
        {
            return _retryPolicy.RunOperation(operation, _operationTimeout);
        }
    }
}
