namespace MassTransit.Monitoring.Health
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;


    public static class BusHealthExtensions
    {
        public static async Task<BusHealthStatus> WaitForHealthStatus(this IBusHealth healthChecks, BusHealthStatus expectedStatus, TimeSpan timeout)
        {
            var expiresAt = DateTime.UtcNow + timeout;

            var result = healthChecks.CheckHealth();
            while (result.Status != expectedStatus)
            {
                if (DateTime.UtcNow >= expiresAt)
                    return result.Status;

                await Task.Delay(100);

                result = healthChecks.CheckHealth();
            }

            return result.Status;
        }

        public static Task<BusHealthStatus[]> WaitForHealthStatuses(this IEnumerable<IBusHealth> healthChecks, BusHealthStatus expectedStatus,
            TimeSpan timeout)
        {
            return Task.WhenAll(healthChecks.Select(healthCheck => WaitForHealthStatus(healthCheck, expectedStatus, timeout)));
        }
    }
}
