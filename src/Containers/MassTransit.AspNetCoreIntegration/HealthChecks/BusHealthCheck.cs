namespace MassTransit.AspNetCoreIntegration.HealthChecks
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Monitoring.Health;


    public class BusHealthCheck :
        IHealthCheck
    {
        readonly IBusHealth _busHealth;

        public BusHealthCheck(IBusHealth healthCheck)
        {
            _busHealth = healthCheck;
        }

        Task<HealthCheckResult> IHealthCheck.CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
        {
            var result = _busHealth.CheckHealth();

            return Task.FromResult(result.Status switch
            {
                BusHealthStatus.Healthy => HealthCheckResult.Healthy(result.Description, result.Data),
                BusHealthStatus.Degraded => HealthCheckResult.Degraded(result.Description, result.Exception, result.Data),
                _ => HealthCheckResult.Unhealthy(result.Description, result.Exception, result.Data)
            });
        }
    }
}
