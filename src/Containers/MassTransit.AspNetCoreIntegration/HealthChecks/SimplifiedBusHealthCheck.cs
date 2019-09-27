namespace MassTransit.AspNetCoreIntegration.HealthChecks
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Diagnostics.HealthChecks;


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

        public void ReportBusStarted()
        {
            _busStarted = true;
        }
    }
}
