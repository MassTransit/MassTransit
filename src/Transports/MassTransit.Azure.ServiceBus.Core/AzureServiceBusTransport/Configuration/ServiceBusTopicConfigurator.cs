namespace MassTransit.AzureServiceBusTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using Azure.Messaging.ServiceBus.Administration;
    using Topology;


    public class ServiceBusTopicConfigurator :
        ServiceBusMessageEntityConfigurator,
        IServiceBusTopicConfigurator
    {
        public ServiceBusTopicConfigurator(string topicPath, bool temporary)
            : base(topicPath)
        {
            if (temporary)
                AutoDeleteOnIdle = Defaults.TemporaryAutoDeleteOnIdle;
        }

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

            if (MaxSizeInMegabytes.HasValue)
                options.MaxSizeInMegabytes = MaxSizeInMegabytes.Value;

            if (MaxMessageSizeInKilobytes.HasValue)
                options.MaxMessageSizeInKilobytes = MaxMessageSizeInKilobytes;

            if (RequiresDuplicateDetection.HasValue)
                options.RequiresDuplicateDetection = RequiresDuplicateDetection.Value;

            if (!string.IsNullOrWhiteSpace(UserMetadata))
                options.UserMetadata = UserMetadata;

            return options;
        }
    }
}
