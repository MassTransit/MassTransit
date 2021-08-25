using System;

namespace MassTransit.Transports.Outbox.Configuration
{
    public class OnRampTransportOptions : IOnRampTransportOptions
    {
        // Cluster Settings
        public bool Clustered { get; set; } = false;
        public TimeSpan ClusterCheckinInterval { get; set; } = TimeSpan.FromMilliseconds(7500);
        public TimeSpan ClusterCheckInMisfireInterval { get; set; } = TimeSpan.FromMilliseconds(7500);
        public TimeSpan ClusterDbRetryInterval { get; set; } = TimeSpan.FromSeconds(15);
        public int ClusterRetryableActionErrorLogThreshold { get; set; } = 4;

        // Sweeper Settings
        public int PrefetchCount { get; set; } = 10; // make default larger
        public int SendAttemptThreshold { get; set; } = 5;
        public TimeSpan SweeperPollingInterval { get; set; } = TimeSpan.FromMilliseconds(2000);
        public bool BulkRemove { get; set; } = false;

        // ## Shared Settings
        public string OnRampName { get; set; } = "MT-OnRamp";

        public bool DisableServices { get; set; } = false;
    }
}
