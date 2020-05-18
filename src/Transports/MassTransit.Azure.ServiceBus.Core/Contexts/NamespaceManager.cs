namespace MassTransit.Azure.ServiceBus.Core.Contexts
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.Management;
    using Microsoft.Azure.ServiceBus.Primitives;
    using Pipeline;


    public class NamespaceManager
    {
        readonly ManagementClient _managementClient;

        public NamespaceManager(Uri address, NamespaceManagerSettings settings)
        {
            _managementClient = new ManagementClient(address.ToString(), settings.TokenProvider);
            Address = address;
            Settings = settings;
        }

        public Uri Address { get; }
        public NamespaceManagerSettings Settings { get; }

        public Task<QueueDescription> GetQueueAsync(string path)
        {
            return RunOperation(() => _managementClient.GetQueueAsync(path));
        }

        public Task<bool> QueueExistsAsync(string path)
        {
            return RunOperation(() => _managementClient.QueueExistsAsync(path));
        }

        public Task<QueueDescription> CreateQueueAsync(QueueDescription queueDescription)
        {
            return RunOperation(() => _managementClient.CreateQueueAsync(queueDescription));
        }

        public Task<TopicDescription> GetTopicAsync(string path)
        {
            return RunOperation(() => _managementClient.GetTopicAsync(path));
        }

        public Task<bool> TopicExistsAsync(string path)
        {
            return RunOperation(() => _managementClient.TopicExistsAsync(path));
        }

        public Task<TopicDescription> CreateTopicAsync(TopicDescription topicDescription)
        {
            return RunOperation(() => _managementClient.CreateTopicAsync(topicDescription));
        }

        public Task<bool> SubscriptionExistsAsync(string topicPath, string subscriptionName)
        {
            return RunOperation(() => _managementClient.SubscriptionExistsAsync(topicPath, subscriptionName));
        }

        public Task<RuleDescription> GetRuleAsync(string topicPath, string subscriptionName, string ruleName)
        {
            return RunOperation(() => _managementClient.GetRuleAsync(topicPath, subscriptionName, ruleName));
        }

        public Task<SubscriptionDescription> GetSubscriptionAsync(string topicPath, string subscriptionName)
        {
            return RunOperation(() => _managementClient.GetSubscriptionAsync(topicPath, subscriptionName));
        }

        public Task DeleteSubscriptionAsync(string topicPath, string subscriptionName)
        {
            return RunOperation(() => _managementClient.DeleteSubscriptionAsync(topicPath, subscriptionName));
        }

        public Task<SubscriptionDescription> UpdateSubscriptionAsync(SubscriptionDescription description)
        {
            return RunOperation(() => _managementClient.UpdateSubscriptionAsync(description));
        }

        public Task<RuleDescription> UpdateRuleAsync(string topicPath, string subscriptionName, RuleDescription ruleDescription)
        {
            return RunOperation(() => _managementClient.UpdateRuleAsync(topicPath, subscriptionName, ruleDescription));
        }

        public Task<SubscriptionDescription> CreateSubscriptionAsync(SubscriptionDescription description, RuleDescription rule)
        {
            return RunOperation(() => _managementClient.CreateSubscriptionAsync(description, rule));
        }

        public Task<SubscriptionDescription> CreateSubscriptionAsync(SubscriptionDescription description, Filter filter)
        {
            var ruleDescription = new RuleDescription(Guid.NewGuid().ToString(), filter);
            return RunOperation(() => _managementClient.CreateSubscriptionAsync(description, ruleDescription));
        }

        public Task<SubscriptionDescription> CreateSubscriptionAsync(SubscriptionDescription description)
        {
            return RunOperation(() => _managementClient.CreateSubscriptionAsync(description));
        }

        public static NamespaceManager CreateFromConnectionString(string connectionString)
        {
            var connectionStringBuilder = new ServiceBusConnectionStringBuilder(connectionString);
            var tokenProvider = CreateTokenProvider(connectionStringBuilder);

            var uri = new Uri(connectionStringBuilder.Endpoint);
            var settings = new NamespaceManagerSettings {TokenProvider = tokenProvider};

            return new NamespaceManager(uri, settings);
        }

        public static ITokenProvider CreateTokenProvider(ServiceBusConnectionStringBuilder builder)
        {
            if (builder.SasToken != null)
                return TokenProvider.CreateSharedAccessSignatureTokenProvider(builder.SasToken);

            if (builder.SasKeyName != null && builder.SasKey != null)
                return TokenProvider.CreateSharedAccessSignatureTokenProvider(builder.SasKeyName, builder.SasKey);

            throw new Exception(
                "Could not create token provider. Either ITokenProvider has to be passed into constructor or connection string should contain information such as SAS token / SAS key name and SAS key.");
        }

        async Task<T> RunOperation<T>(Func<Task<T>> operation)
        {
            T result = default;
            var task = Settings.RetryPolicy.RunOperation(
                async () =>
                {
                    result = await operation().ConfigureAwait(false);
                }, Settings.OperationTimeout);

            await task.ConfigureAwait(false);

            return result;
        }

        Task RunOperation(Func<Task> operation)
        {
            return Settings.RetryPolicy.RunOperation(
                async () =>
                {
                    await operation().ConfigureAwait(false);
                }, Settings.OperationTimeout);
        }
    }
}
