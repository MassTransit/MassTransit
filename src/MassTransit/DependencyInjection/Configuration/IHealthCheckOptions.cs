#nullable enable
namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Diagnostics.HealthChecks;


    public interface IHealthCheckOptions
    {
        /// <summary>
        /// Set the health check name, overrides the default bus type name
        /// </summary>
        public string? Name { get; }

        /// <summary>
        /// The <see cref="HealthStatus" /> that should be reported when the health check fails.
        /// If null then the default status of <see cref="HealthStatus.Unhealthy" /> will be reported.
        /// </summary>
        [Obsolete("Use MinimalFailureStatus instead.", true)]
        public HealthStatus? FailureStatus { get; }

        /// <summary>
        /// The minimal <see cref="HealthStatus" /> that should be reported when the health check fails.
        /// If null then all statuses from <see cref="HealthStatus.Unhealthy"/> to <see cref="HealthStatus.Healthy"/> will be reported depending on app health.
        /// </summary>
        public HealthStatus? MinimalFailureStatus { get; }

        /// <summary>
        /// A list of tags that can be used to filter sets of health checks
        /// </summary>
        public HashSet<string> Tags { get; }
    }
}
