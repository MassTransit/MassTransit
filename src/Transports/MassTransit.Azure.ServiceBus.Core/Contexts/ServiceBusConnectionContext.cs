namespace MassTransit.Azure.ServiceBus.Core.Contexts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using global::Azure.Messaging.ServiceBus;
    using global::Azure.Messaging.ServiceBus.Administration;
    using GreenPipes;
    using Internals.Extensions;
    using MassTransit.Azure.ServiceBus.Core.Transport;
    using Transports;


    public class ServiceBusConnectionContext :
        BasePipeContext,
        ConnectionContext,
        IAsyncDisposable
    {
        readonly ServiceBusClient _connection;
        readonly ServiceBusAdministrationClient _managementClient;

        public ServiceBusConnectionContext(ServiceBusClient connection, ServiceBusAdministrationClient managementClient,
            CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _connection = connection;
            _managementClient = managementClient;
        }

        public Uri Endpoint => new Uri($"amqps://{_connection.FullyQualifiedNamespace}");

        public (ServiceBusProcessor, ServiceBusSessionProcessor) CreateQueueClient(ReceiveSettings settings)
        {
            return
                (_connection.CreateProcessor(
                    settings.Path,
                    new ServiceBusProcessorOptions
                    {
                        AutoCompleteMessages = false,
                        PrefetchCount = settings.PrefetchCount,
                        MaxConcurrentCalls = settings.MaxConcurrentCalls,
                        MaxAutoLockRenewalDuration = settings.MaxAutoRenewDuration,
                        ReceiveMode = ServiceBusReceiveMode.PeekLock,
                    }),
                _connection.CreateSessionProcessor(
                    settings.Path,
                    new ServiceBusSessionProcessorOptions
                    {
                        AutoCompleteMessages = false,
                        PrefetchCount = settings.PrefetchCount,
                        MaxAutoLockRenewalDuration = settings.MaxAutoRenewDuration,
                        ReceiveMode = ServiceBusReceiveMode.PeekLock,
                    }));
        }

        public (ServiceBusProcessor, ServiceBusSessionProcessor) CreateSubscriptionClient(SubscriptionSettings settings)
        {
            return
                (_connection.CreateProcessor(
                    settings.TopicDescription.Name, settings.SubscriptionDescription.SubscriptionName,
                    new ServiceBusProcessorOptions
                    {
                        AutoCompleteMessages = false,
                        PrefetchCount = settings.PrefetchCount,
                        MaxConcurrentCalls = settings.MaxConcurrentCalls,
                        MaxAutoLockRenewalDuration = settings.MaxAutoRenewDuration,
                        ReceiveMode = ServiceBusReceiveMode.PeekLock,
                    }),
                _connection.CreateSessionProcessor(
                    settings.TopicDescription.Name, settings.SubscriptionDescription.SubscriptionName,
                    new ServiceBusSessionProcessorOptions
                    {
                        AutoCompleteMessages = false,
                        PrefetchCount = settings.PrefetchCount,
                        MaxAutoLockRenewalDuration = settings.MaxAutoRenewDuration,
                        ReceiveMode = ServiceBusReceiveMode.PeekLock,
                    }));
        }

        public ServiceBusSender CreateMessageSender(string entityPath)
        {
            return _connection.CreateSender(entityPath);
        }

        public async Task<QueueProperties> CreateQueue(CreateQueueOptions queueDescription)
        {
            QueueProperties queueProperties;
            var queueExists = await QueueExistsAsync(queueDescription.Name).ConfigureAwait(false);
            if (queueExists)
                queueProperties = await GetQueueAsync(queueDescription.Name).ConfigureAwait(false);
            else
            {
                try
                {
                    TransportLogMessages.CreateQueue(queueDescription.Name);

                    queueProperties = await CreateQueueAsync(queueDescription).ConfigureAwait(false);
                }
                catch (ServiceBusException sbe) when (sbe.Reason == ServiceBusFailureReason.MessagingEntityAlreadyExists)
                {
                    queueProperties = await GetQueueAsync(queueDescription.Name).ConfigureAwait(false);
                }
            }

            LogContext.Debug?.Log("Queue: {Queue} ({Attributes})", queueDescription.Name,
                string.Join(", ",
                    new[]
                    {
                        queueDescription.RequiresDuplicateDetection ? "dupe detect" : "",
                        queueDescription.DeadLetteringOnMessageExpiration ? "dead letter" : "",
                        queueDescription.RequiresSession ? "session" : "",
                        queueDescription.AutoDeleteOnIdle != Defaults.AutoDeleteOnIdle
                            ? $"auto-delete: {queueDescription.AutoDeleteOnIdle.ToFriendlyString()}"
                            : ""
                    }.Where(x => !string.IsNullOrWhiteSpace(x))));

            return queueProperties;
        }

        public async Task<TopicProperties> CreateTopic(CreateTopicOptions topicDescription)
        {
            TopicProperties topicProperties;
            var topicExists = await TopicExistsAsync(topicDescription.Name).ConfigureAwait(false);
            if (topicExists)
                topicProperties = await GetTopicAsync(topicDescription.Name).ConfigureAwait(false);
            else
            {
                try
                {
                    TransportLogMessages.CreateTopic(topicDescription.Name);

                    topicProperties = await CreateTopicAsync(topicDescription).ConfigureAwait(false);
                }
                catch(ServiceBusException e) when (e.Reason == ServiceBusFailureReason.MessagingEntityAlreadyExists)
                {
                    topicProperties = await GetTopicAsync(topicDescription.Name).ConfigureAwait(false);
                }
            }

            LogContext.Debug?.Log("Topic: {Topic} ({Attributes})", topicDescription.Name,
                string.Join(", ",
                    new[]
                    {
                        topicDescription.RequiresDuplicateDetection ? "dupe detect" : "",
                        topicDescription.EnablePartitioning ? "partitioned" : "",
                        topicDescription.SupportOrdering ? "ordered" : ""
                    }.Where(x => !string.IsNullOrWhiteSpace(x))));

            return topicProperties;
        }

        public async Task<SubscriptionProperties> CreateTopicSubscription(CreateSubscriptionOptions description, CreateRuleOptions rule, RuleFilter filter)
        {
            var create = true;
            SubscriptionProperties subscriptionDescription = null;
            try
            {
                subscriptionDescription = await GetSubscriptionAsync(description.TopicName, description.SubscriptionName).ConfigureAwait(false);

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
                        subscriptionDescription.TopicName, subscriptionDescription.ForwardTo);

                    await UpdateSubscriptionAsync(subscriptionDescription).ConfigureAwait(false);
                }

                if (rule != null)
                {
                    var ruleDescription = await GetRuleAsync(description.TopicName, description.SubscriptionName, rule.Name).ConfigureAwait(false);
                    if (rule.Name == ruleDescription.Name && (rule.Filter != ruleDescription.Filter || rule.Action != ruleDescription.Action))
                    {
                        LogContext.Debug?.Log("Updating subscription Rule: {Rule} ({DescriptionFilter} -> {Filter})", rule.Name,
                            ruleDescription.Filter.ToString(), rule.Filter.ToString());

                        await UpdateRuleAsync(description.TopicName, description.SubscriptionName, ruleDescription).ConfigureAwait(false);
                    }
                }
                else if (filter != null)
                {
                    IList<RuleProperties> rules = await GetRulesAsync(description.TopicName, description.SubscriptionName).ConfigureAwait(false);
                    if (rules.Count == 1)
                    {
                        var existingRule = rules[0];

                        if (Guid.TryParse(existingRule.Name, out _) && existingRule.Filter != filter)
                        {
                            LogContext.Debug?.Log("Updating subscription filter: {Rule} ({DescriptionFilter} -> {Filter})", existingRule.Name,
                                existingRule.Filter.ToString(), filter.ToString());

                            existingRule.Filter = filter;

                            await UpdateRuleAsync(description.TopicName, description.SubscriptionName, existingRule).ConfigureAwait(false);
                        }
                    }
                }

                create = false;
            }
            catch (ServiceBusException e) when (e.Reason == ServiceBusFailureReason.MessagingEntityNotFound)
            {
            }

            if (create)
            {
                var created = false;
                try
                {
                    LogContext.Debug?.Log("Creating subscription {Subscription} {Topic} -> {ForwardTo}", description.SubscriptionName, description.TopicName,
                        description.ForwardTo);

                    subscriptionDescription = rule != null
                        ? await CreateSubscriptionAsync(description, rule).ConfigureAwait(false)
                        : filter != null
                            ? await CreateSubscriptionAsync(description, filter).ConfigureAwait(false)
                            : await CreateSubscriptionAsync(description).ConfigureAwait(false);

                    created = true;
                }
                catch (ServiceBusException e) when (e.Reason == ServiceBusFailureReason.MessagingEntityAlreadyExists)
                {
                }

                if (!created)
                {
                    subscriptionDescription = await GetSubscriptionAsync(description.TopicName, description.SubscriptionName)
                        .ConfigureAwait(false);
                }
            }

            LogContext.Debug?.Log("Subscription {Subscription} ({Topic} -> {ForwardTo})", subscriptionDescription.SubscriptionName,
                subscriptionDescription.TopicName, subscriptionDescription.ForwardTo);

            return subscriptionDescription;
        }

        public async Task DeleteTopicSubscription(CreateSubscriptionOptions description)
        {
            try
            {
                await DeleteSubscriptionAsync(description.TopicName, description.SubscriptionName).ConfigureAwait(false);

                LogContext.Debug?.Log("Subscription Deleted: {Subscription} ({Topic} -> {ForwardTo})", description.SubscriptionName, description.TopicName,
                    description.ForwardTo);
            }
            catch (ServiceBusException ex) when (ex.Reason == ServiceBusFailureReason.MessagingEntityNotFound)
            {
            }
            catch (Exception ex)
            {
                LogContext.Error?.Log(ex, "Subscription Delete Faulted: {Subscription} ({Topic} -> {ForwardTo})", description.SubscriptionName,
                    description.TopicName, description.ForwardTo);
            }
        }

        public async ValueTask DisposeAsync()
        {
            var address = _connection.FullyQualifiedNamespace;

            TransportLogMessages.DisconnectHost(address);

            await _connection.DisposeAsync().ConfigureAwait(false);

            TransportLogMessages.DisconnectedHost(address);
        }

        Task<QueueProperties> GetQueueAsync(string path)
        {
            return RunOperation(async () => (await _managementClient.GetQueueAsync(path)).Value);
        }

        Task<bool> QueueExistsAsync(string path)
        {
            return RunOperation(async () => (await _managementClient.QueueExistsAsync(path)).Value);
        }

        Task<QueueProperties> CreateQueueAsync(CreateQueueOptions queueDescription)
        {
            return RunOperation(async () => (await _managementClient.CreateQueueAsync(queueDescription)).Value);
        }

        Task<TopicProperties> GetTopicAsync(string path)
        {
            return RunOperation(async () => (await _managementClient.GetTopicAsync(path)).Value);
        }

        Task<bool> TopicExistsAsync(string path)
        {
            return RunOperation(async () => (await _managementClient.TopicExistsAsync(path)).Value);
        }

        Task<TopicProperties> CreateTopicAsync(CreateTopicOptions topicDescription)
        {
            return RunOperation(async () => (await _managementClient.CreateTopicAsync(topicDescription)).Value);
        }

        Task<SubscriptionProperties> GetSubscriptionAsync(string topicPath, string subscriptionName)
        {
            return RunOperation(async () => (await _managementClient.GetSubscriptionAsync(topicPath, subscriptionName)).Value);
        }

        Task DeleteSubscriptionAsync(string topicPath, string subscriptionName)
        {
            return RunOperation(() => _managementClient.DeleteSubscriptionAsync(topicPath, subscriptionName));
        }

        Task<SubscriptionProperties> UpdateSubscriptionAsync(SubscriptionProperties description)
        {
            return RunOperation(async () => (await _managementClient.UpdateSubscriptionAsync(description)).Value);
        }

        Task<RuleProperties> GetRuleAsync(string topicPath, string subscriptionName, string ruleName)
        {
            return RunOperation(async () => (await _managementClient.GetRuleAsync(topicPath, subscriptionName, ruleName)).Value);
        }

        Task<IList<RuleProperties>> GetRulesAsync(string topicPath, string subscriptionName)
        {
            return RunOperation(() => _managementClient.GetRulesAsync(topicPath, subscriptionName).ToListAsync());
        }

        Task<RuleProperties> UpdateRuleAsync(string topicPath, string subscriptionName, RuleProperties ruleDescription)
        {
            return RunOperation(async () => (await _managementClient.UpdateRuleAsync(topicPath, subscriptionName, ruleDescription)).Value);
        }

        Task<SubscriptionProperties> CreateSubscriptionAsync(CreateSubscriptionOptions description, CreateRuleOptions rule)
        {
            return RunOperation(async () => (await _managementClient.CreateSubscriptionAsync(description, rule)).Value);
        }

        Task<SubscriptionProperties> CreateSubscriptionAsync(CreateSubscriptionOptions description, RuleFilter filter)
        {
            var ruleDescription = new CreateRuleOptions(NewId.NextGuid().ToString(), filter);
            return RunOperation(async () => (await _managementClient.CreateSubscriptionAsync(description, ruleDescription)).Value);
        }

        Task<SubscriptionProperties> CreateSubscriptionAsync(CreateSubscriptionOptions description)
        {
            return RunOperation(async () => (await _managementClient.CreateSubscriptionAsync(description)).Value);
        }

        async Task<T> RunOperation<T>(Func<Task<T>> operation)
        {
            T result = default;
            result = await operation().ConfigureAwait(false);
            return result;
        }

        Task RunOperation(Func<Task> operation)
        {
            return operation();
        }
    }
}
