namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.ServiceBus;
    using Azure.Messaging.ServiceBus.Administration;
    using Internals;
    using MassTransit.Middleware;
    using Transports;


    public class ServiceBusConnectionContext :
        BasePipeContext,
        ConnectionContext,
        IAsyncDisposable
    {
        readonly ServiceBusAdministrationClient _administrationClient;
        readonly ServiceBusClient _client;

        public ServiceBusConnectionContext(ServiceBusClient client, ServiceBusAdministrationClient administrationClient, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _client = client;
            _administrationClient = administrationClient;
            Endpoint = new Uri($"sb://{_client.FullyQualifiedNamespace}");
        }

        public Uri Endpoint { get; }

        public ServiceBusProcessor CreateQueueProcessor(ReceiveSettings settings)
        {
            return _client.CreateProcessor(settings.Path, GetProcessorOptions(settings));
        }

        public ServiceBusSessionProcessor CreateQueueSessionProcessor(ReceiveSettings settings)
        {
            return _client.CreateSessionProcessor(settings.Path, GetSessionProcessorOptions(settings));
        }

        public ServiceBusProcessor CreateSubscriptionProcessor(SubscriptionSettings settings)
        {
            return _client.CreateProcessor(settings.CreateTopicOptions.Name, settings.CreateSubscriptionOptions.SubscriptionName,
                GetProcessorOptions(settings));
        }

        public ServiceBusSessionProcessor CreateSubscriptionSessionProcessor(SubscriptionSettings settings)
        {
            return _client.CreateSessionProcessor(settings.CreateTopicOptions.Name, settings.CreateSubscriptionOptions.SubscriptionName,
                GetSessionProcessorOptions(settings));
        }

        public ServiceBusSender CreateMessageSender(string entityPath)
        {
            return _client.CreateSender(entityPath);
        }

        public async Task<QueueProperties> CreateQueue(CreateQueueOptions createQueueOptions)
        {
            QueueProperties queueProperties;
            var queueExists = await QueueExistsAsync(createQueueOptions.Name).ConfigureAwait(false);
            if (queueExists)
                queueProperties = await GetQueueAsync(createQueueOptions.Name).ConfigureAwait(false);
            else
            {
                try
                {
                    TransportLogMessages.CreateQueue(createQueueOptions.Name);

                    queueProperties = await CreateQueueAsync(createQueueOptions).ConfigureAwait(false);
                }
                catch (ServiceBusException sbe) when (sbe.Reason == ServiceBusFailureReason.MessagingEntityAlreadyExists)
                {
                    queueProperties = await GetQueueAsync(createQueueOptions.Name).ConfigureAwait(false);
                }
            }

            LogContext.Debug?.Log("Queue: {Queue} ({Attributes})", createQueueOptions.Name,
                string.Join(", ",
                    new[]
                    {
                        createQueueOptions.RequiresDuplicateDetection ? "dupe detect" : "",
                        createQueueOptions.DeadLetteringOnMessageExpiration ? "dead letter" : "",
                        createQueueOptions.RequiresSession ? "session" : "",
                        createQueueOptions.AutoDeleteOnIdle != Defaults.AutoDeleteOnIdle
                            ? $"auto-delete: {createQueueOptions.AutoDeleteOnIdle.ToFriendlyString()}"
                            : ""
                    }.Where(x => !string.IsNullOrWhiteSpace(x))));

            return queueProperties;
        }

        public async Task<TopicProperties> CreateTopic(CreateTopicOptions createTopicOptions)
        {
            TopicProperties topicProperties;
            var topicExists = await TopicExistsAsync(createTopicOptions.Name).ConfigureAwait(false);
            if (topicExists)
                topicProperties = await GetTopicAsync(createTopicOptions.Name).ConfigureAwait(false);
            else
            {
                try
                {
                    TransportLogMessages.CreateTopic(createTopicOptions.Name);

                    topicProperties = await CreateTopicAsync(createTopicOptions).ConfigureAwait(false);
                }
                catch (ServiceBusException e) when (e.Reason == ServiceBusFailureReason.MessagingEntityAlreadyExists)
                {
                    topicProperties = await GetTopicAsync(createTopicOptions.Name).ConfigureAwait(false);
                }
            }

            LogContext.Debug?.Log("Topic: {Topic} ({Attributes})", createTopicOptions.Name,
                string.Join(", ",
                    new[]
                    {
                        createTopicOptions.RequiresDuplicateDetection ? "dupe detect" : "",
                        createTopicOptions.EnablePartitioning ? "partitioned" : "",
                        createTopicOptions.SupportOrdering ? "ordered" : ""
                    }.Where(x => !string.IsNullOrWhiteSpace(x))));

            return topicProperties;
        }

        public async Task<SubscriptionProperties> CreateTopicSubscription(CreateSubscriptionOptions createSubscriptionOptions, CreateRuleOptions rule,
            RuleFilter filter)
        {
            var create = true;
            SubscriptionProperties subscriptionProperties = null;
            try
            {
                subscriptionProperties = await GetSubscriptionAsync(createSubscriptionOptions.TopicName, createSubscriptionOptions.SubscriptionName)
                    .ConfigureAwait(false);

                string NormalizeForwardTo(string forwardTo)
                {
                    return string.IsNullOrEmpty(forwardTo)
                        ? string.Empty
                        : forwardTo.Replace(Endpoint.ToString(), string.Empty).Trim('/');
                }

                var targetForwardTo = NormalizeForwardTo(createSubscriptionOptions.ForwardTo);
                var currentForwardTo = NormalizeForwardTo(subscriptionProperties.ForwardTo);

                if (!targetForwardTo.Equals(currentForwardTo))
                {
                    LogContext.Debug?.Log("Updating subscription: {Subscription} ({Topic} -> {ForwardTo})", subscriptionProperties.SubscriptionName,
                        subscriptionProperties.TopicName, subscriptionProperties.ForwardTo);

                    await UpdateSubscriptionAsync(subscriptionProperties).ConfigureAwait(false);
                }

                if (rule != null)
                {
                    var ruleProperties = await GetRuleAsync(createSubscriptionOptions.TopicName, createSubscriptionOptions.SubscriptionName, rule.Name)
                        .ConfigureAwait(false);
                    if (rule.Name == ruleProperties.Name && (rule.Filter != ruleProperties.Filter || rule.Action != ruleProperties.Action))
                    {
                        ruleProperties.Filter = rule.Filter;
                        ruleProperties.Action = rule.Action;

                        LogContext.Debug?.Log("Updating subscription Rule: {Rule} ({DescriptionFilter} -> {Filter})", rule.Name,
                            ruleProperties.Filter.ToString(), rule.Filter.ToString());

                        await UpdateRuleAsync(createSubscriptionOptions.TopicName, createSubscriptionOptions.SubscriptionName, ruleProperties)
                            .ConfigureAwait(false);
                    }
                }
                else if (filter != null)
                {
                    IList<RuleProperties> rules = await GetRulesAsync(createSubscriptionOptions.TopicName, createSubscriptionOptions.SubscriptionName)
                        .ConfigureAwait(false);
                    if (rules.Count == 1)
                    {
                        var existingRule = rules[0];

                        if (Guid.TryParse(existingRule.Name, out _) && existingRule.Filter != filter)
                        {
                            LogContext.Debug?.Log("Updating subscription filter: {Rule} ({DescriptionFilter} -> {Filter})", existingRule.Name,
                                existingRule.Filter.ToString(), filter.ToString());

                            existingRule.Filter = filter;

                            await UpdateRuleAsync(createSubscriptionOptions.TopicName, createSubscriptionOptions.SubscriptionName, existingRule)
                                .ConfigureAwait(false);
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
                    LogContext.Debug?.Log("Creating subscription {Subscription} {Topic} -> {ForwardTo}", createSubscriptionOptions.SubscriptionName,
                        createSubscriptionOptions.TopicName,
                        createSubscriptionOptions.ForwardTo);

                    subscriptionProperties = rule != null
                        ? await CreateSubscriptionAsync(createSubscriptionOptions, rule).ConfigureAwait(false)
                        : filter != null
                            ? await CreateSubscriptionAsync(createSubscriptionOptions, filter).ConfigureAwait(false)
                            : await CreateSubscriptionAsync(createSubscriptionOptions).ConfigureAwait(false);

                    created = true;
                }
                catch (ServiceBusException e) when (e.Reason == ServiceBusFailureReason.MessagingEntityAlreadyExists)
                {
                }

                if (!created)
                {
                    subscriptionProperties = await GetSubscriptionAsync(createSubscriptionOptions.TopicName, createSubscriptionOptions.SubscriptionName)
                        .ConfigureAwait(false);
                }
            }

            LogContext.Debug?.Log("Subscription {Subscription} ({Topic} -> {ForwardTo})", subscriptionProperties.SubscriptionName,
                subscriptionProperties.TopicName, subscriptionProperties.ForwardTo);

            return subscriptionProperties;
        }

        public async Task DeleteTopicSubscription(CreateSubscriptionOptions subscriptionOptions)
        {
            try
            {
                await DeleteSubscriptionAsync(subscriptionOptions).ConfigureAwait(false);

                LogContext.Debug?.Log("Subscription Deleted: {Subscription} {Topic}", subscriptionOptions.SubscriptionName, subscriptionOptions.TopicName);
            }
            catch (ServiceBusException ex) when (ex.Reason == ServiceBusFailureReason.MessagingEntityNotFound)
            {
            }
            catch (Exception ex)
            {
                LogContext.Error?.Log(ex, "Subscription Delete Faulted: {Subscription} {Topic}", subscriptionOptions.SubscriptionName,
                    subscriptionOptions.TopicName);
            }
        }

        public async ValueTask DisposeAsync()
        {
            var address = _client.FullyQualifiedNamespace;

            TransportLogMessages.DisconnectHost(address);

            await _client.DisposeAsync().ConfigureAwait(false);

            TransportLogMessages.DisconnectedHost(address);
        }

        static ServiceBusSessionProcessorOptions GetSessionProcessorOptions(ClientSettings settings)
        {
            return new ServiceBusSessionProcessorOptions
            {
                AutoCompleteMessages = false,
                PrefetchCount = settings.PrefetchCount,
                MaxAutoLockRenewalDuration = settings.MaxAutoRenewDuration,
                ReceiveMode = ServiceBusReceiveMode.PeekLock,
                MaxConcurrentSessions = settings.MaxConcurrentCalls,
                MaxConcurrentCallsPerSession = settings.MaxConcurrentCallsPerSession,
                SessionIdleTimeout = settings.SessionIdleTimeout
            };
        }

        static ServiceBusProcessorOptions GetProcessorOptions(ClientSettings settings)
        {
            return new ServiceBusProcessorOptions
            {
                AutoCompleteMessages = false,
                PrefetchCount = settings.PrefetchCount,
                MaxAutoLockRenewalDuration = settings.MaxAutoRenewDuration,
                ReceiveMode = ServiceBusReceiveMode.PeekLock,
                MaxConcurrentCalls = settings.MaxConcurrentCalls,
            };
        }

        Task<QueueProperties> GetQueueAsync(string path)
        {
            return RunOperation(async () => (await _administrationClient.GetQueueAsync(path)).Value);
        }

        Task<bool> QueueExistsAsync(string path)
        {
            return RunOperation(async () => (await _administrationClient.QueueExistsAsync(path)).Value);
        }

        Task<QueueProperties> CreateQueueAsync(CreateQueueOptions createQueueOptions)
        {
            return RunOperation(async () => (await _administrationClient.CreateQueueAsync(createQueueOptions)).Value);
        }

        Task<TopicProperties> GetTopicAsync(string path)
        {
            return RunOperation(async () => (await _administrationClient.GetTopicAsync(path)).Value);
        }

        Task<bool> TopicExistsAsync(string path)
        {
            return RunOperation(async () => (await _administrationClient.TopicExistsAsync(path)).Value);
        }

        Task<TopicProperties> CreateTopicAsync(CreateTopicOptions createTopicOptions)
        {
            return RunOperation(async () => (await _administrationClient.CreateTopicAsync(createTopicOptions)).Value);
        }

        Task<SubscriptionProperties> GetSubscriptionAsync(string topicPath, string subscriptionName)
        {
            return RunOperation(async () => (await _administrationClient.GetSubscriptionAsync(topicPath, subscriptionName)).Value);
        }

        Task DeleteSubscriptionAsync(CreateSubscriptionOptions subscriptionOptions)
        {
            return RunOperation(() => _administrationClient.DeleteSubscriptionAsync(subscriptionOptions.TopicName, subscriptionOptions.SubscriptionName));
        }

        Task<SubscriptionProperties> UpdateSubscriptionAsync(SubscriptionProperties subscriptionProperties)
        {
            return RunOperation(async () => (await _administrationClient.UpdateSubscriptionAsync(subscriptionProperties)).Value);
        }

        Task<RuleProperties> GetRuleAsync(string topicPath, string subscriptionName, string ruleName)
        {
            return RunOperation(async () => (await _administrationClient.GetRuleAsync(topicPath, subscriptionName, ruleName)).Value);
        }

        Task<IList<RuleProperties>> GetRulesAsync(string topicPath, string subscriptionName)
        {
            return RunOperation(() => _administrationClient.GetRulesAsync(topicPath, subscriptionName).ToListAsync());
        }

        Task<RuleProperties> UpdateRuleAsync(string topicPath, string subscriptionName, RuleProperties ruleProperties)
        {
            return RunOperation(async () => (await _administrationClient.UpdateRuleAsync(topicPath, subscriptionName, ruleProperties)).Value);
        }

        Task<SubscriptionProperties> CreateSubscriptionAsync(CreateSubscriptionOptions createSubscriptionOptions, CreateRuleOptions rule)
        {
            return RunOperation(async () => (await _administrationClient.CreateSubscriptionAsync(createSubscriptionOptions, rule)).Value);
        }

        Task<SubscriptionProperties> CreateSubscriptionAsync(CreateSubscriptionOptions createSubscriptionOptions, RuleFilter filter)
        {
            var createRuleOptions = new CreateRuleOptions(NewId.NextGuid().ToString(), filter);
            return RunOperation(async () => (await _administrationClient.CreateSubscriptionAsync(createSubscriptionOptions, createRuleOptions)).Value);
        }

        Task<SubscriptionProperties> CreateSubscriptionAsync(CreateSubscriptionOptions createSubscriptionOptions)
        {
            return RunOperation(async () => (await _administrationClient.CreateSubscriptionAsync(createSubscriptionOptions)).Value);
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
