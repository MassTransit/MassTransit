namespace MassTransit.Testing.Implementations
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Util;


    /// <summary>
    /// An activity indicator for receive endpoint queues. Utilizes a timer that restarts on receive activity.
    /// </summary>
    public class BusActivityReceiveIndicator : BaseBusActivityIndicatorConnectable,
        ISignalResource,
        IReceiveObserver
    {
        readonly RollingTimer _receiveIdleTimer;
        readonly ISignalResource _signalResource;
        int _activityStarted;

        public BusActivityReceiveIndicator(ISignalResource signalResource, TimeSpan receiveIdleTimeout)
        {
            _signalResource = signalResource;
            _receiveIdleTimer = new RollingTimer(SignalInactivity, receiveIdleTimeout);
        }

        public BusActivityReceiveIndicator(ISignalResource signalResource)
            :
            this(signalResource, TimeSpan.FromSeconds(5))
        {
        }

        public BusActivityReceiveIndicator(TimeSpan receiveIdleTimeout)
            :
            this(null, receiveIdleTimeout)
        {
        }

        public BusActivityReceiveIndicator()
            :
            this(null)
        {
        }

        public override bool IsMet =>
            _receiveIdleTimer.Triggered ||
            Interlocked.CompareExchange(ref _activityStarted, int.MinValue, int.MinValue) == 0;

        Task IReceiveObserver.PreReceive(ReceiveContext context)
        {
            Interlocked.CompareExchange(ref _activityStarted, 1, 0);
            _receiveIdleTimer.Restart();
            return Task.CompletedTask;
        }

        Task IReceiveObserver.PostReceive(ReceiveContext context)
        {
            _receiveIdleTimer.Restart();
            return Task.CompletedTask;
        }

        Task IReceiveObserver.PostConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
        {
            return Task.CompletedTask;
        }

        Task IReceiveObserver.ConsumeFault<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
        {
            return Task.CompletedTask;
        }

        Task IReceiveObserver.ReceiveFault(ReceiveContext context, Exception exception)
        {
            _receiveIdleTimer.Restart();
            return Task.CompletedTask;
        }

        public void Signal()
        {
            SignalInactivity(null);
        }

        void SignalInactivity(object state)
        {
            _signalResource?.Signal();
            ConditionUpdated();
            Interlocked.CompareExchange(ref _activityStarted, 0, 1);
            _receiveIdleTimer.Stop();
        }
    }
}
