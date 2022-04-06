namespace MassTransit.AzureServiceBusTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using Azure.Messaging.ServiceBus.Administration;
    using Topology;


    public class ServiceBusQueueConfigurator :
        ServiceBusMessageEntityConfigurator,
        IServiceBusQueueConfigurator
    {
        public ServiceBusQueueConfigurator(string path)
            : base(path)
        {
            EnableDeadLetteringOnMessageExpiration = true;
            LockDuration = Defaults.LockDuration;
            MaxDeliveryCount = 5;
        }

        public bool? EnableDeadLetteringOnMessageExpiration { get; set; }

        public bool? EnableDeadLetteringOnFilterEvaluationExceptions { get; set; }

        public string ForwardDeadLetteredMessagesTo { get; set; }

        public string ForwardTo { get; set; }

        public TimeSpan? LockDuration { get; set; }

        public int? MaxDeliveryCount { get; set; }

        public bool? RequiresSession { get; set; }

        public int? MaxConcurrentCallsPerSession { get; set; }

        public IEnumerable<ValidationResult> Validate()
        {
            if (!ServiceBusEntityNameValidator.Validator.IsValidEntityName(Path))
                yield return this.Failure("Path", $"must be a valid queue path: {Path}");

            if (AutoDeleteOnIdle.HasValue && AutoDeleteOnIdle != TimeSpan.Zero && AutoDeleteOnIdle < TimeSpan.FromMinutes(5))
                yield return this.Failure("AutoDeleteOnIdle", "must be zero, or >= 5:00");
        }

        public CreateQueueOptions GetCreateQueueOptions()
        {
            var options = new CreateQueueOptions(FullPath);

            if (AutoDeleteOnIdle.HasValue)
                options.AutoDeleteOnIdle = AutoDeleteOnIdle.Value;

            if (DefaultMessageTimeToLive.HasValue)
                options.DefaultMessageTimeToLive = DefaultMessageTimeToLive.Value;

            if (DuplicateDetectionHistoryTimeWindow.HasValue)
                options.DuplicateDetectionHistoryTimeWindow = DuplicateDetectionHistoryTimeWindow.Value;

            if (EnableBatchedOperations.HasValue)
                options.EnableBatchedOperations = EnableBatchedOperations.Value;

            if (EnableDeadLetteringOnMessageExpiration.HasValue)
                options.DeadLetteringOnMessageExpiration = EnableDeadLetteringOnMessageExpiration.Value;

            if (EnablePartitioning.HasValue)
                options.EnablePartitioning = EnablePartitioning.Value;

            if (!string.IsNullOrWhiteSpace(ForwardDeadLetteredMessagesTo))
                options.ForwardDeadLetteredMessagesTo = ForwardDeadLetteredMessagesTo;

            if (!string.IsNullOrWhiteSpace(ForwardTo))
                options.ForwardTo = ForwardTo;

            if (LockDuration.HasValue)
                options.LockDuration = LockDuration.Value;

            if (MaxDeliveryCount.HasValue)
                options.MaxDeliveryCount = MaxDeliveryCount.Value;

            if (MaxSizeInMegabytes.HasValue)
                options.MaxSizeInMegabytes = MaxSizeInMegabytes.Value;

            if (MaxMessageSizeInKilobytes.HasValue)
                options.MaxMessageSizeInKilobytes = MaxMessageSizeInKilobytes;

            if (RequiresDuplicateDetection.HasValue)
                options.RequiresDuplicateDetection = RequiresDuplicateDetection.Value;

            if (RequiresSession.HasValue)
                options.RequiresSession = RequiresSession.Value;

            if (!string.IsNullOrWhiteSpace(UserMetadata))
                options.UserMetadata = UserMetadata;

            return options;
        }

        public Uri GetQueueAddress(Uri hostAddress)
        {
            return new ServiceBusEndpointAddress(hostAddress, Path, AutoDeleteOnIdle);
        }
    }
}
