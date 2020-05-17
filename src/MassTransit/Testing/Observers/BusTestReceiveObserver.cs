namespace MassTransit.Testing.Observers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Util;


    public class BusTestReceiveObserver :
        InactivityTestObserver,
        IReceiveObserver
    {
        readonly RollingTimer _inactivityTimer;
        int _activityDetected;

        public BusTestReceiveObserver(TimeSpan inactivityTimout)
        {
            _inactivityTimer = new RollingTimer(OnActivityTimeout, inactivityTimout);
        }

        public override bool IsInactive => _inactivityTimer.Triggered || Interlocked.CompareExchange(ref _activityDetected, int.MinValue, int.MinValue) == 0;

        public Task PreReceive(ReceiveContext context)
        {
            Interlocked.CompareExchange(ref _activityDetected, 1, 0);
            _inactivityTimer.Restart();

            return TaskUtil.Completed;
        }

        public Task PostReceive(ReceiveContext context)
        {
            _inactivityTimer.Restart();

            return TaskUtil.Completed;
        }

        public Task PostConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
            where T : class
        {
            return TaskUtil.Completed;
        }

        public Task ConsumeFault<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
            where T : class
        {
            return TaskUtil.Completed;
        }

        public Task ReceiveFault(ReceiveContext context, Exception exception)
        {
            _inactivityTimer.Restart();

            return TaskUtil.Completed;
        }

        void OnActivityTimeout(object state)
        {
            Task.Run(() => NotifyInactive());

            _inactivityTimer.Stop();
            Interlocked.CompareExchange(ref _activityDetected, 0, 1);
        }
    }
}
