namespace MassTransit.Monitoring.Health
{
    using System;
    using System.Threading.Tasks;


    public static class BusHealthExtensions
    {
        public static async Task<BusHealthStatus> WaitForHealthStatus(this IBusHealth healthChecks, BusHealthStatus expectedStatus, TimeSpan timeout)
        {
            DateTime expiresAt = DateTime.UtcNow + timeout;

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
    }
}
