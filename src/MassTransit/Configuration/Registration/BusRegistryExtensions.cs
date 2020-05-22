namespace MassTransit.Registration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public static class BusRegistryExtensions
    {
        public static async Task Start(this IBusDepot depot, TimeSpan timeout)
        {
            using var cancellationTokenSource = new CancellationTokenSource(timeout);

            await depot.Start(cancellationTokenSource.Token).ConfigureAwait(false);
        }

        public static async Task Stop(this IBusDepot depot, TimeSpan timeout)
        {
            using var cancellationTokenSource = new CancellationTokenSource(timeout);

            await depot.Stop(cancellationTokenSource.Token).ConfigureAwait(false);
        }
    }
}
