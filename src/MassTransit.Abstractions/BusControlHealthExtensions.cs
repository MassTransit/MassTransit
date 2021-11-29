namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;


    public static class BusControlHealthExtensions
    {
        public static async Task<BusHealthStatus> WaitForHealthStatus(this IBusControl busControl, BusHealthStatus expectedStatus, TimeSpan timeout)
        {
            var expiresAt = DateTime.UtcNow + timeout;

            var result = busControl.CheckHealth();
            while (result.Status != expectedStatus)
            {
                if (DateTime.UtcNow >= expiresAt)
                    return result.Status;

                await Task.Delay(100).ConfigureAwait(false);

                result = busControl.CheckHealth();
            }

            return result.Status;
        }

        public static Task<BusHealthStatus[]> WaitForHealthStatus(this IEnumerable<IBusControl> busControls, BusHealthStatus expectedStatus, TimeSpan timeout)
        {
            return Task.WhenAll(busControls.Select(healthCheck => WaitForHealthStatus(healthCheck, expectedStatus, timeout)));
        }

        public static async Task<BusHealthStatus> WaitForHealthStatus(this IBusControl busControl, BusHealthStatus expectedStatus,
            CancellationToken cancellationToken)
        {
            var result = busControl.CheckHealth();
            while (result.Status != expectedStatus)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await Task.Delay(100, cancellationToken).ConfigureAwait(false);

                result = busControl.CheckHealth();
            }

            return result.Status;
        }

        public static Task<BusHealthStatus[]> WaitForHealthStatus(this IEnumerable<IBusControl> busControls, BusHealthStatus expectedStatus,
            CancellationToken cancellationToken)
        {
            return Task.WhenAll(busControls.Select(healthCheck => WaitForHealthStatus(healthCheck, expectedStatus, cancellationToken)));
        }
    }
}
