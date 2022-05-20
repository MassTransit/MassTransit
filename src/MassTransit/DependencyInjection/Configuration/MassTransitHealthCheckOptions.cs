#nullable enable
namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Diagnostics.HealthChecks;


    public class MassTransitHealthCheckOptions<TBus> :
        IHealthCheckOptionsConfigurator,
        IHealthCheckOptions
        where TBus : IBus
    {
        public MassTransitHealthCheckOptions()
        {
            Tags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// The health check name. If null the type name of bus instance will be used
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The <see cref="HealthStatus" /> that should be reported when the health check fails.
        /// If null then the default status of <see cref="HealthStatus.Unhealthy" /> will be reported.
        /// </summary>
        public HealthStatus? FailureStatus { get; set; }

        /// <summary>
        /// A list of tags that can be used to filter sets of health checks. If empty, the default tags
        /// will be used.
        /// </summary>
        public HashSet<string> Tags { get; }
    }
}
