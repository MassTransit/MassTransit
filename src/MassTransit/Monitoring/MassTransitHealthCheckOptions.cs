namespace MassTransit.Monitoring
{
    using System.Collections.Generic;
    using Microsoft.Extensions.Diagnostics.HealthChecks;

    public class MassTransitHealthCheckOptions
    {
        /// <summary>
        /// The health check name. Optional. If null the type name of bus instance will be used
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The <see cref="HealthStatus"/> that should be reported when the health check fails.
        /// If null then the default status of <see cref="HealthStatus.Unhealthy"/> will be reported.
        /// </summary>
        public HealthStatus? FailureStatus { get; set; }

        /// <summary>
        /// A list of tags that can be used to filter sets of health checks
        /// </summary>
        public IEnumerable<string> Tags { get; set; }
    }
}
