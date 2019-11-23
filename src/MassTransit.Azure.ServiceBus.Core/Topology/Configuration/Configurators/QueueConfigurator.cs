namespace MassTransit.Azure.ServiceBus.Core.Topology.Configuration.Configurators
{
    using System;
    using System.Collections.Generic;
    using GreenPipes;
    using Microsoft.Azure.ServiceBus.Management;


    public class QueueConfigurator :
        MessageEntityConfigurator,
        IQueueConfigurator
    {
        public QueueConfigurator(string path)
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

        public IEnumerable<ValidationResult> Validate()
        {
            if (!ServiceBusEntityNameValidator.Validator.IsValidEntityName(Path))
                yield return this.Failure("Path", $"must be a valid queue path: {Path}");

            if (AutoDeleteOnIdle.HasValue && AutoDeleteOnIdle != TimeSpan.Zero && AutoDeleteOnIdle < TimeSpan.FromMinutes(5))
                yield return this.Failure("AutoDeleteOnIdle", "must be zero, or >= 5:00");
        }

        public QueueDescription GetQueueDescription()
        {
            var description = new QueueDescription(FullPath);

            if (AutoDeleteOnIdle.HasValue)
                description.AutoDeleteOnIdle = AutoDeleteOnIdle.Value;

            if (DefaultMessageTimeToLive.HasValue)
                description.DefaultMessageTimeToLive = DefaultMessageTimeToLive.Value;

            if (DuplicateDetectionHistoryTimeWindow.HasValue)
                description.DuplicateDetectionHistoryTimeWindow = DuplicateDetectionHistoryTimeWindow.Value;

            if (EnableBatchedOperations.HasValue)
                description.EnableBatchedOperations = EnableBatchedOperations.Value;

            if (EnableDeadLetteringOnMessageExpiration.HasValue)
                description.EnableDeadLetteringOnMessageExpiration = EnableDeadLetteringOnMessageExpiration.Value;

            if (EnablePartitioning.HasValue)
                description.EnablePartitioning = EnablePartitioning.Value;

            if (!string.IsNullOrWhiteSpace(ForwardDeadLetteredMessagesTo))
                description.ForwardDeadLetteredMessagesTo = ForwardDeadLetteredMessagesTo;

            if (!string.IsNullOrWhiteSpace(ForwardTo))
                description.ForwardTo = ForwardTo;

            if (LockDuration.HasValue)
                description.LockDuration = LockDuration.Value;

            if (MaxDeliveryCount.HasValue)
                description.MaxDeliveryCount = MaxDeliveryCount.Value;

            if (MaxSizeInMB.HasValue)
                description.MaxSizeInMB = MaxSizeInMB.Value;

            if (RequiresDuplicateDetection.HasValue)
                description.RequiresDuplicateDetection = RequiresDuplicateDetection.Value;

            if (RequiresSession.HasValue)
                description.RequiresSession = RequiresSession.Value;

            if (!string.IsNullOrWhiteSpace(UserMetadata))
                description.UserMetadata = UserMetadata;

            return description;
        }

        public Uri GetQueueAddress(Uri hostAddress)
        {
            return new ServiceBusEndpointAddress(hostAddress, Path, AutoDeleteOnIdle);
        }
    }
}
