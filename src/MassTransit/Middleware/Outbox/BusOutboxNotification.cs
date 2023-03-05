namespace MassTransit.Middleware.Outbox
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;


    public class BusOutboxNotification :
        IBusOutboxNotification
    {
        readonly object _lock = new object();
        readonly IOptions<OutboxDeliveryServiceOptions> _options;
        CancellationTokenSource _cancellationTokenSource;

        public BusOutboxNotification(IOptions<OutboxDeliveryServiceOptions> options)
        {
            _options = options;
        }

        public async Task WaitForDelivery(CancellationToken cancellationToken)
        {
            lock (_lock)
                _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            try
            {
                var delay = await Task.Delay(_options.Value.QueryDelay, _cancellationTokenSource.Token)
                    .ContinueWith(t => t, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default)
                    .ConfigureAwait(false);

                if (delay.IsCanceled)
                    cancellationToken.ThrowIfCancellationRequested();
            }
            finally
            {
                lock (_lock)
                {
                    _cancellationTokenSource.Dispose();
                    _cancellationTokenSource = null;
                }
            }
        }

        public void Delivered()
        {
            lock (_lock)
                _cancellationTokenSource?.Cancel();
        }
    }
}
