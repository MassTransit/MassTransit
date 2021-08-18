namespace MassTransit.Azure.ServiceBus.Core
{
    using System;
    using System.ComponentModel;
    using Microsoft.Azure.ServiceBus.Management;


    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class Defaults
    {
        public static TimeSpan LockDuration { get; set; } = TimeSpan.FromMinutes(5);
        public static TimeSpan DefaultMessageTimeToLive { get; set; } = TimeSpan.FromDays(365 + 1);
        public static TimeSpan BasicMessageTimeToLive { get; set; } = TimeSpan.FromDays(14);

        public static TimeSpan AutoDeleteOnIdle { get; set; } = TimeSpan.FromDays(427);
        public static TimeSpan TemporaryAutoDeleteOnIdle { get; set; } = TimeSpan.FromMinutes(5);
        public static TimeSpan MaxAutoRenewDuration { get; set; } = TimeSpan.FromMinutes(5);
        public static TimeSpan MessageWaitTimeout { get; set; } = TimeSpan.FromSeconds(10);
        public static TimeSpan ShutdownTimeout { get; set; } = TimeSpan.FromMilliseconds(100);

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
    }
}
