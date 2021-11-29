namespace MassTransit.Testing.Implementations
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Util;


    /// <summary>
    /// An activity indicator for publish endpoints. Utilizes a timer that restarts on publish activity.
    /// </summary>
    public class BusActivityPublishIndicator : BaseBusActivityIndicatorConnectable,
        ISignalResource
    {
        readonly RollingTimer _receiveIdleTimer;
        readonly ISignalResource _signalResource;
        int _activityStarted;

        public BusActivityPublishIndicator(ISignalResource signalResource, TimeSpan receiveIdleTimeout)
        {
            _signalResource = signalResource;
            _receiveIdleTimer = new RollingTimer(SignalInactivity, receiveIdleTimeout);
        }

        public BusActivityPublishIndicator(ISignalResource signalResource)
            :
            this(signalResource, TimeSpan.FromSeconds(5))
        {
        }

        public BusActivityPublishIndicator(TimeSpan receiveIdleTimeout)
            :
            this(null, receiveIdleTimeout)
        {
        }

        public BusActivityPublishIndicator()
            :
            this(null)
        {
        }

        public override bool IsMet =>
            _receiveIdleTimer.Triggered ||
            Interlocked.CompareExchange(ref _activityStarted, int.MinValue, int.MinValue) == 0;

        public void Signal()
        {
            SignalInactivity(null);
        }

        public Task PrePublish<T>(PublishContext<T> context)
            where T : class
        {
            Interlocked.CompareExchange(ref _activityStarted, 1, 0);
            _receiveIdleTimer.Restart();
            return Task.CompletedTask;
        }

        public Task PostPublish<T>(PublishContext<T> context)
            where T : class
        {
            _receiveIdleTimer.Restart();
            return Task.CompletedTask;
        }

        public Task PublishFault<T>(PublishContext<T> context, Exception exception)
            where T : class
        {
            _receiveIdleTimer.Restart();
            return Task.CompletedTask;
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
