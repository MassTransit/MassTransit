namespace MassTransit.Azure.ServiceBus.Core
{
    using System;
    using System.ComponentModel;
    using Microsoft.Azure.ServiceBus.Management;


    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class Defaults
    {
        public static TimeSpan LockDuration = TimeSpan.FromMinutes(5);
        public static TimeSpan DefaultMessageTimeToLive = TimeSpan.FromDays(365 + 1);
        public static TimeSpan BasicMessageTimeToLive = TimeSpan.FromDays(14);

        public static TimeSpan AutoDeleteOnIdle => TimeSpan.FromDays(427);
        public static TimeSpan TemporaryAutoDeleteOnIdle => TimeSpan.FromMinutes(5);
        public static TimeSpan MaxAutoRenewDuration => TimeSpan.FromMinutes(5);
        public static TimeSpan MessageWaitTimeout => TimeSpan.FromSeconds(10);
        public static int MaxConcurrentCalls => Math.Max(Environment.ProcessorCount, 8);
        public static int PrefetchCount => Math.Max(MaxConcurrentCalls, 32);

        public static QueueDescription CreateQueueDescription(string queueName)
        {
            return new QueueDescription(queueName)
            {
                AutoDeleteOnIdle = AutoDeleteOnIdle,
                DefaultMessageTimeToLive = DefaultMessageTimeToLive,
                EnableBatchedOperations = true,
                EnableDeadLetteringOnMessageExpiration = true,
                LockDuration = LockDuration,
                MaxDeliveryCount = 5
            };
        }

        public static TopicDescription CreateTopicDescription(string topicName)
        {
            return new TopicDescription(topicName)
            {
                AutoDeleteOnIdle = AutoDeleteOnIdle,
                DefaultMessageTimeToLive = DefaultMessageTimeToLive,
                EnableBatchedOperations = true
            };
        }

        public static SubscriptionDescription CreateSubscriptionDescription(string topicPath, string subscriptionName, QueueDescription queue, string queuePath)
        {
            return new SubscriptionDescription(topicPath, subscriptionName)
            {
                AutoDeleteOnIdle = queue.AutoDeleteOnIdle,
                DefaultMessageTimeToLive = queue.DefaultMessageTimeToLive,
                EnableBatchedOperations = queue.EnableBatchedOperations,
                EnableDeadLetteringOnMessageExpiration = queue.EnableDeadLetteringOnMessageExpiration,
                ForwardTo = queuePath,
                LockDuration = queue.LockDuration,
                MaxDeliveryCount = queue.MaxDeliveryCount,
                UserMetadata = queue.UserMetadata
            };
        }

        public static SubscriptionDescription CreateSubscriptionDescription(string topicPath, string subscriptionName)
        {
            var queue = CreateQueueDescription(subscriptionName);

            return new SubscriptionDescription(topicPath, subscriptionName)
            {
                AutoDeleteOnIdle = queue.AutoDeleteOnIdle,
                DefaultMessageTimeToLive = queue.DefaultMessageTimeToLive,
                EnableBatchedOperations = queue.EnableBatchedOperations,
                EnableDeadLetteringOnMessageExpiration = queue.EnableDeadLetteringOnMessageExpiration,
                LockDuration = queue.LockDuration,
                MaxDeliveryCount = queue.MaxDeliveryCount,
                UserMetadata = queue.UserMetadata
            };
        }
    }
}
