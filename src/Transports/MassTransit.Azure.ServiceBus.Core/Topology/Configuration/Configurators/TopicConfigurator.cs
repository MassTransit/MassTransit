namespace MassTransit.Azure.ServiceBus.Core.Topology.Configurators
{
    using System;
    using System.Collections.Generic;
    using global::Azure.Messaging.ServiceBus.Administration;
    using GreenPipes;


    public class TopicConfigurator :
        MessageEntityConfigurator,
        ITopicConfigurator
    {
        public TopicConfigurator(string topicPath, bool temporary)
            : base(topicPath)
        {
            if (temporary)
                AutoDeleteOnIdle = Defaults.TemporaryAutoDeleteOnIdle;
        }

        public bool? EnableFilteringMessagesBeforePublishing { get; set; }

        public IEnumerable<ValidationResult> Validate()
        {
            if (!ServiceBusEntityNameValidator.Validator.IsValidEntityName(Path))
                yield return this.Failure("Path", $"must be a valid topic path: {Path}");

            if (AutoDeleteOnIdle.HasValue && AutoDeleteOnIdle != TimeSpan.Zero && AutoDeleteOnIdle < TimeSpan.FromMinutes(5))
                yield return this.Failure("AutoDeleteOnIdle", "must be zero, or >= 5:00");
        }

        public CreateTopicOptions GetCreateTopicOptions()
        {
            var options = new CreateTopicOptions(FullPath);

            if (AutoDeleteOnIdle.HasValue)
                options.AutoDeleteOnIdle = AutoDeleteOnIdle.Value;

            if (DefaultMessageTimeToLive.HasValue)
                options.DefaultMessageTimeToLive = DefaultMessageTimeToLive.Value;

            if (DuplicateDetectionHistoryTimeWindow.HasValue)
                options.DuplicateDetectionHistoryTimeWindow = DuplicateDetectionHistoryTimeWindow.Value;

            if (EnableBatchedOperations.HasValue)
                options.EnableBatchedOperations = EnableBatchedOperations.Value;

            if (EnablePartitioning.HasValue)
                options.EnablePartitioning = EnablePartitioning.Value;

            if (MaxSizeInMB.HasValue)
                options.MaxSizeInMegabytes = MaxSizeInMB.Value;

            if (RequiresDuplicateDetection.HasValue)
                options.RequiresDuplicateDetection = RequiresDuplicateDetection.Value;

            if (!string.IsNullOrWhiteSpace(UserMetadata))
                options.UserMetadata = UserMetadata;

            return options;
        }
    }
}
