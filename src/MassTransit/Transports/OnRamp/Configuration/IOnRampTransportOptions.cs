using System;

namespace MassTransit.Transports.Outbox.Configuration
{
    /// <summary>
    /// Options needed for the Cluster and Sweeper
    /// </summary>
    public interface IOnRampTransportOptions : IOnRampOptions
    {

        // ## HostedServices Settings ##
        /// <summary>
        /// Set to true if you plan all cluster and sweeper services to run in a separate process. Default false
        /// </summary>
        bool DisableServices { get; }


        // Cluster Settings
        bool Clustered { get; }
        TimeSpan ClusterCheckinInterval { get; }
        TimeSpan ClusterCheckInMisfireInterval { get; }
        TimeSpan ClusterDbRetryInterval { get; }
        int ClusterRetryableActionErrorLogThreshold { get; }

        // Sweeper Settings
        int PrefetchCount { get; }
        int SendAttemptThreshold { get; }
        TimeSpan SweeperPollingInterval { get; }
        bool BulkRemove { get; }
    }
}
