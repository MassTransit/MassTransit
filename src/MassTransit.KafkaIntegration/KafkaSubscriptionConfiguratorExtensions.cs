namespace MassTransit.KafkaIntegration
{
    using System;
    using Confluent.Kafka;


    public static class KafkaSubscriptionConfiguratorExtensions
    {
        public static void UseConfig(this IKafkaSubscriptionConfigurator configurator, ConsumerConfig config)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            configurator.AutoOffsetReset = config.AutoOffsetReset;
            configurator.GroupId = config.GroupId;
            configurator.GroupInstanceId = config.GroupInstanceId;
            configurator.PartitionAssignmentStrategy = config.PartitionAssignmentStrategy;
            configurator.GroupProtocolType = config.GroupProtocolType;
            configurator.EnableAutoOffsetStore = config.EnableAutoOffsetStore;
            configurator.QueuedMinMessages = config.QueuedMinMessages;
            configurator.QueuedMaxMessagesKbytes = config.QueuedMaxMessagesKbytes;

            configurator.EnablePartitionEof = config.EnablePartitionEof;
            configurator.CheckCrcs = config.CheckCrcs;

            if (config.SessionTimeoutMs.HasValue)
                configurator.SessionTimeout = TimeSpan.FromMilliseconds(config.SessionTimeoutMs.Value);
            if (config.HeartbeatIntervalMs.HasValue)
                configurator.HeartbeatInterval = TimeSpan.FromMilliseconds(config.HeartbeatIntervalMs.Value);

            if (config.CoordinatorQueryIntervalMs.HasValue)
                configurator.CoordinatorQueryInterval = TimeSpan.FromMilliseconds(config.CoordinatorQueryIntervalMs.Value);
            if (config.MaxPollIntervalMs.HasValue)
                configurator.MaxPollInterval = TimeSpan.FromMilliseconds(config.MaxPollIntervalMs.Value);

            if (config.IsolationLevel.HasValue)
                configurator.UseIsolationLevel(config.IsolationLevel.Value);
            if (config.EnableAutoCommit.HasValue)
                configurator.EnableAutoCommit(TimeSpan.FromMilliseconds(config.AutoCommitIntervalMs ?? 5000));
            else
                configurator.DisableAutoCommit();

            configurator.ConfigureFetch(cfg =>
            {
                if (config.FetchErrorBackoffMs.HasValue)
                    cfg.ErrorBackoffInterval = TimeSpan.FromMilliseconds(config.FetchErrorBackoffMs.Value);
                if (config.FetchWaitMaxMs.HasValue)
                    cfg.WaitMaxInterval = TimeSpan.FromMilliseconds(config.FetchWaitMaxMs.Value);

                cfg.MaxPartitionBytes = config.MaxPartitionFetchBytes;
                cfg.MaxBytes = config.FetchMaxBytes;
                cfg.MinBytes = config.FetchMinBytes;
            });
        }
    }
}
