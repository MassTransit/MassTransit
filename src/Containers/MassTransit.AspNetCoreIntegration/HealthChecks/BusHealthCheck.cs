namespace MassTransit.AspNetCoreIntegration.HealthChecks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Registration;


    public class BusHealthCheck :
        IHealthCheck
    {
        readonly IBusInstance _busInstance;

        public BusHealthCheck(IBusInstance busInstance)
        {
            _busInstance = busInstance;
        }

        Task<HealthCheckResult> IHealthCheck.CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
        {
            var result = _busInstance.BusControl.CheckHealth();

            var data = new Dictionary<string, object>
            {
                ["Endpoints"] = result.Endpoints.ToDictionary(x => x.Key,
                    x => new
                    {
                        Status = Enum.GetName(typeof(BusHealthStatus), x.Value.Status),
                        x.Value.Description
                    })
            };

            return Task.FromResult(result.Status switch
            {
                BusHealthStatus.Healthy => HealthCheckResult.Healthy(result.Description, data),
                BusHealthStatus.Degraded => HealthCheckResult.Degraded(result.Description, result.Exception, data),
                _ => HealthCheckResult.Unhealthy(result.Description, result.Exception, data)
            });
        }
    }
}
