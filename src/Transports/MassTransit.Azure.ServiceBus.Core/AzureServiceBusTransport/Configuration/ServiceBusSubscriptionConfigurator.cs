namespace MassTransit.AzureServiceBusTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using Azure.Messaging.ServiceBus.Administration;
    using Topology;


    public class ServiceBusSubscriptionConfigurator :
        ServiceBusEndpointEntityConfigurator,
        IServiceBusSubscriptionConfigurator
    {
        public ServiceBusSubscriptionConfigurator(string subscriptionName, string topicPath)
        {
            TopicPath = topicPath;
            SubscriptionName = subscriptionName;
        }

        public bool? EnableDeadLetteringOnFilterEvaluationExceptions { private get; set; }

        public RuleFilter Filter { get; set; }
        public CreateRuleOptions Rule { get; set; }

        public string ForwardTo { private get; set; }

        public string TopicPath { get; }

        public string SubscriptionName { get; }

        public IEnumerable<ValidationResult> Validate()
        {
            if (!ServiceBusEntityNameValidator.Validator.IsValidEntityName(TopicPath))
                yield return this.Failure("TopicPath", $"must be a valid topic path: {TopicPath}");

            if (!ServiceBusSubscriptionNameValidator.Validator.IsValidEntityName(SubscriptionName))
                yield return this.Failure("SubscriptionName", $"must be a valid subscription name: {SubscriptionName}");

            if (AutoDeleteOnIdle.HasValue && AutoDeleteOnIdle != TimeSpan.Zero && AutoDeleteOnIdle < TimeSpan.FromMinutes(5))
                yield return this.Failure("AutoDeleteOnIdle", "must be zero, or >= 5:00");

            if (Rule != null && Filter != null)
                yield return this.Failure("Rule/Filter", "only a rule or a filter may be specified");
        }

        public CreateSubscriptionOptions GetCreateSubscriptionOptions()
        {
            var options = new CreateSubscriptionOptions(TopicPath, SubscriptionName);

            if (AutoDeleteOnIdle.HasValue)
                options.AutoDeleteOnIdle = AutoDeleteOnIdle.Value;

            if (DefaultMessageTimeToLive.HasValue)
                options.DefaultMessageTimeToLive = DefaultMessageTimeToLive.Value;

            if (EnableBatchedOperations.HasValue)
                options.EnableBatchedOperations = EnableBatchedOperations.Value;

            if (EnableDeadLetteringOnFilterEvaluationExceptions.HasValue)
                options.EnableDeadLetteringOnFilterEvaluationExceptions = EnableDeadLetteringOnFilterEvaluationExceptions.Value;

            if (EnableDeadLetteringOnMessageExpiration.HasValue)
                options.DeadLetteringOnMessageExpiration = EnableDeadLetteringOnMessageExpiration.Value;

            if (!string.IsNullOrWhiteSpace(ForwardDeadLetteredMessagesTo))
                options.ForwardDeadLetteredMessagesTo = ForwardDeadLetteredMessagesTo;

            if (!string.IsNullOrWhiteSpace(ForwardTo))
                options.ForwardTo = ForwardTo;

            if (LockDuration.HasValue)
                options.LockDuration = LockDuration.Value;

            if (MaxDeliveryCount.HasValue)
                options.MaxDeliveryCount = MaxDeliveryCount.Value;

            if (RequiresSession.HasValue)
                options.RequiresSession = RequiresSession.Value;

            if (!string.IsNullOrWhiteSpace(UserMetadata))
                options.UserMetadata = UserMetadata;

            return options;
        }
    }
}
