namespace MassTransit.Testing.Implementations
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Internals;

    /// <summary>
    /// Signalable resource which monitors bus activity.
    /// </summary>
    public class BusActivityMonitor : ISignalResource,
        IBusActivityMonitor
    {
        readonly SemaphoreSlim _activityEvent = new SemaphoreSlim(0, 1);
        readonly Func<bool> _busInactivePredicate;
        readonly object _lockObj = new object();
        Task _pendingAwaitTask = Task.CompletedTask;

        public BusActivityMonitor()
        { }

        public BusActivityMonitor(Func<bool> busInactivePredicate)
        {
            _busInactivePredicate = busInactivePredicate;
        }

        public Task AwaitBusInactivity()
        {
            return AwaitBusInactivity(CancellationToken.None);
        }

        public async Task<bool> AwaitBusInactivity(TimeSpan timeout)
        {
            try
            {
                using (var cts = new CancellationTokenSource(timeout))
                    await AwaitBusInactivity(cts.Token);
                return true;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
        }

        public async Task AwaitBusInactivity(CancellationToken cancellationToken)
        {
            if (_busInactivePredicate is null)
            {
                // Don't wanted to break anything for people who used the class directly, so if _busInactive is null we falling back to the old implementation
                await _activityEvent.WaitAsync(cancellationToken);
                return;
            }

            while (!_busInactivePredicate())
                await WaitSemaphoreInternal().OrCanceled(cancellationToken);
        }

        void ISignalResource.Signal()
        {
            try
            {
                _activityEvent.Release();
            }
            catch (SemaphoreFullException)
            {
            }
        }

        Task WaitSemaphoreInternal()
        {
            if (_busInactivePredicate is null)
                return _activityEvent.WaitAsync();

            lock (_lockObj)
            {
                if (!_busInactivePredicate() && _pendingAwaitTask.IsCompleted)
                {
                    // We don't want to call the semaphore wait function multiple times, so it's safe to call the monitor from multiple threads
                    _pendingAwaitTask = _activityEvent.WaitAsync();
                }

                return _pendingAwaitTask;
            }
        }
    }
}
