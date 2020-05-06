namespace MassTransit.Registration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public static class BusRegistryExtensions
    {
        public static async Task Start(this IBusRegistry registry, TimeSpan timeout)
        {
            using var cancellationTokenSource = new CancellationTokenSource(timeout);

            await registry.Start(cancellationTokenSource.Token).ConfigureAwait(false);
        }

        public static async Task Stop(this IBusRegistry registry, TimeSpan timeout)
        {
            using var cancellationTokenSource = new CancellationTokenSource(timeout);

            await registry.Stop(cancellationTokenSource.Token).ConfigureAwait(false);
        }
    }
}
