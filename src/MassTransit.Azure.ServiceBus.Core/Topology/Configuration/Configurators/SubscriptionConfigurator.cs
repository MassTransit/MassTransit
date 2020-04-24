namespace MassTransit.Azure.ServiceBus.Core.Topology.Configurators
{
    using System;
    using System.Collections.Generic;
    using GreenPipes;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.Management;


    public class SubscriptionConfigurator :
        EndpointEntityConfigurator,
        ISubscriptionConfigurator
    {
        public SubscriptionConfigurator(string topicPath, string subscriptionName)
        {
            TopicPath = topicPath;
            SubscriptionName = subscriptionName;
        }

        public bool? EnableDeadLetteringOnFilterEvaluationExceptions { private get; set; }

        public Filter Filter { get; set; }
        public RuleDescription Rule { get; set; }

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

        public SubscriptionDescription GetSubscriptionDescription()
        {
            var description = new SubscriptionDescription(TopicPath, SubscriptionName);

            if (AutoDeleteOnIdle.HasValue)
                description.AutoDeleteOnIdle = AutoDeleteOnIdle.Value;

            if (DefaultMessageTimeToLive.HasValue)
                description.DefaultMessageTimeToLive = DefaultMessageTimeToLive.Value;

            if (EnableBatchedOperations.HasValue)
                description.EnableBatchedOperations = EnableBatchedOperations.Value;

            if (EnableDeadLetteringOnFilterEvaluationExceptions.HasValue)
                description.EnableDeadLetteringOnFilterEvaluationExceptions = EnableDeadLetteringOnFilterEvaluationExceptions.Value;

            if (EnableDeadLetteringOnMessageExpiration.HasValue)
                description.EnableDeadLetteringOnMessageExpiration = EnableDeadLetteringOnMessageExpiration.Value;

            if (!string.IsNullOrWhiteSpace(ForwardDeadLetteredMessagesTo))
                description.ForwardDeadLetteredMessagesTo = ForwardDeadLetteredMessagesTo;

            if (!string.IsNullOrWhiteSpace(ForwardTo))
                description.ForwardTo = ForwardTo;

            if (LockDuration.HasValue)
                description.LockDuration = LockDuration.Value;

            if (MaxDeliveryCount.HasValue)
                description.MaxDeliveryCount = MaxDeliveryCount.Value;

            if (RequiresSession.HasValue)
                description.RequiresSession = RequiresSession.Value;

            if (!string.IsNullOrWhiteSpace(UserMetadata))
                description.UserMetadata = UserMetadata;

            return description;
        }
    }
}
