namespace MassTransit.Testing.Implementations
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// Signalable resource which monitors bus activity.
    /// </summary>
    public class BusActivityMonitor : ISignalResource,
        IBusActivityMonitor
    {
        readonly SemaphoreSlim _activityEvent = new SemaphoreSlim(0, 1);

        Task IBusActivityMonitor.AwaitBusInactivity()
        {
            return _activityEvent.WaitAsync();
        }

        Task<bool> IBusActivityMonitor.AwaitBusInactivity(TimeSpan timeout)
        {
            return _activityEvent.WaitAsync(timeout);
        }

        Task IBusActivityMonitor.AwaitBusInactivity(CancellationToken cancellationToken)
        {
            return _activityEvent.WaitAsync(cancellationToken);
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
    }
}
