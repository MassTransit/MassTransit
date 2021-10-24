namespace MassTransit.Azure.ServiceBus.Core
{
    using System;
    using System.ComponentModel;
    using global::Azure.Messaging.ServiceBus.Administration;

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

        public static CreateQueueOptions CreateQueueDescription(string queueName)
        {
            return new CreateQueueOptions(queueName)
            {
                AutoDeleteOnIdle = AutoDeleteOnIdle,
                DefaultMessageTimeToLive = DefaultMessageTimeToLive,
                EnableBatchedOperations = true,
                DeadLetteringOnMessageExpiration = true,
                LockDuration = LockDuration,
                MaxDeliveryCount = 5
            };
        }

        public static CreateTopicOptions CreateTopicDescription(string topicName)
        {
            return new CreateTopicOptions(topicName)
            {
                AutoDeleteOnIdle = AutoDeleteOnIdle,
                DefaultMessageTimeToLive = DefaultMessageTimeToLive,
                EnableBatchedOperations = true
            };
        }
    }
}
