namespace MassTransit.Testing.Implementations
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Util;


    public abstract class InactivityTestObserver :
        Connectable<IInactivityObserver>,
        IDisposable,
        IInactivityObservationSource
    {
        int _activityDetected;
        RollingTimer _inactivityTimer;

        public void Dispose()
        {
            _inactivityTimer?.Dispose();
        }

        public ConnectHandle ConnectInactivityObserver(IInactivityObserver observer)
        {
            var handle = Connect(observer);

            observer.Connected(this);

            return handle;
        }

        public virtual bool IsInactive => _inactivityTimer.Triggered || Interlocked.CompareExchange(ref _activityDetected, int.MinValue, int.MinValue) == 0;

        protected void StartTimer(TimeSpan inactivityTimout)
        {
            _inactivityTimer = new RollingTimer(OnActivityTimeout, inactivityTimout);
        }

        protected Task RestartTimer(bool activityDetected = true)
        {
            if (activityDetected)
                Interlocked.CompareExchange(ref _activityDetected, 1, 0);

            _inactivityTimer.Restart();

            return Task.CompletedTask;
        }

        protected Task NotifyInactive()
        {
            return ForEachAsync(x => x.NoActivity());
        }

        void OnActivityTimeout(object state)
        {
            Task.Run(() => NotifyInactive());

            _inactivityTimer.Stop();
            Interlocked.CompareExchange(ref _activityDetected, 0, 1);
        }
    }
}
