namespace MassTransit
{
    using System.Collections.Generic;
    using Microsoft.Extensions.Diagnostics.HealthChecks;


    public interface IHealthCheckOptionsConfigurator
    {
        /// <summary>
        /// Set the health check name, overrides the default bus type name
        /// </summary>
        public string Name { set; }

        /// <summary>
        /// The <see cref="HealthStatus" /> that should be reported when the health check fails.
        /// If null then the default status of <see cref="HealthStatus.Unhealthy" /> will be reported.
        /// </summary>
        public HealthStatus? FailureStatus { set; }

        /// <summary>
        /// A list of tags that can be used to filter sets of health checks
        /// </summary>
        public HashSet<string> Tags { get; }
    }
}
