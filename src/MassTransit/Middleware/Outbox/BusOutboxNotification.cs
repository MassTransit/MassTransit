namespace MassTransit.Middleware.Outbox
{
    using System;
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
                await Task.Delay(_options.Value.QueryDelay, _cancellationTokenSource.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException e) when (e.CancellationToken == _cancellationTokenSource.Token)
            {
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
