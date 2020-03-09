namespace MassTransit.AspNetCoreIntegration.HealthChecks
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Diagnostics.HealthChecks;


    [Obsolete("Use AddMassTransitHostedService, which requires registered the proper bus health check")]
    public class SimplifiedBusHealthCheck :
        IHealthCheck
    {
        bool _busStarted;

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_busStarted
                ? HealthCheckResult.Healthy("Bus started")
                : new HealthCheckResult(context.Registration.FailureStatus, "Bus not yet started"));
        }

        internal void ReportBusStarted()
        {
            _busStarted = true;
        }

        internal void ReportBusStopped()
        {
            _busStarted = false;
        }
    }
}
