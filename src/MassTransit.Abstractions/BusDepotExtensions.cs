namespace MassTransit
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public static class BusDepotExtensions
    {
        public static async Task Start(this IBusDepot depot, TimeSpan timeout, CancellationToken cancellationToken = default)
        {
            using var timeoutTokenSource = new CancellationTokenSource(timeout);

            if (cancellationToken.CanBeCanceled)
            {
                using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutTokenSource.Token);

                await depot.Start(linkedTokenSource.Token).ConfigureAwait(false);
            }
            else
                await depot.Start(timeoutTokenSource.Token).ConfigureAwait(false);
        }

        public static async Task Stop(this IBusDepot depot, TimeSpan timeout, CancellationToken cancellationToken = default)
        {
            using var timeoutTokenSource = new CancellationTokenSource(timeout);

            if (cancellationToken.CanBeCanceled)
            {
                using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutTokenSource.Token);

                await depot.Stop(linkedTokenSource.Token).ConfigureAwait(false);
            }
            else
                await depot.Stop(timeoutTokenSource.Token).ConfigureAwait(false);
        }
    }
}
