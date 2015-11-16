namespace MassTransit.TestFramework.Indicators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Pipeline;
    using Util;


    /// <summary>
    /// An activity indicator for publish endpoints. Utilizes a timer that restarts on publish activity.
    /// </summary>
    public class BusActivityPublishIndicator : BaseBusActivityIndicatorConnectable, ISignalResource, IPublishObserver
    {
        readonly ISignalResource _signalResource;
        readonly RollingTimer _receiveIdleTimer;
        int _activityStarted = 0;

        public BusActivityPublishIndicator(ISignalResource signalResource, TimeSpan receiveIdleTimeout)
        {
            _signalResource = signalResource;
            _receiveIdleTimer = new RollingTimer(SignalInactivity, receiveIdleTimeout);
        }
        
        public BusActivityPublishIndicator(ISignalResource signalResource) :
            this(signalResource, TimeSpan.FromSeconds(5))
        {
        }

        public BusActivityPublishIndicator(TimeSpan receiveIdleTimeout) :
            this(null, receiveIdleTimeout)
        {
        }
        
        public BusActivityPublishIndicator() :
            this(null)
        {
        }

        Task IPublishObserver.PrePublish<T>(PublishContext<T> context)
        {
            Interlocked.CompareExchange(ref _activityStarted, 1, 0);
            _receiveIdleTimer.Restart();
            return TaskUtil.Completed;
        }

        Task IPublishObserver.PostPublish<T>(PublishContext<T> context)
        {
            _receiveIdleTimer.Restart();
            return TaskUtil.Completed;
        }

        Task IPublishObserver.PublishFault<T>(PublishContext<T> context, Exception exception)
        {
            _receiveIdleTimer.Restart();
            return TaskUtil.Completed;
        }
        

        void SignalInactivity(object state)
        {
            _signalResource?.Signal();
            ConditionUpdated();
            Interlocked.CompareExchange(ref _activityStarted, 0, 1);
            _receiveIdleTimer.Stop();
        }

        public void Signal()
        {
            SignalInactivity(null);
        }


        public override bool State => _receiveIdleTimer.Triggered ||
            Interlocked.CompareExchange(ref _activityStarted, int.MinValue, int.MinValue) == 0;
        

    }
}