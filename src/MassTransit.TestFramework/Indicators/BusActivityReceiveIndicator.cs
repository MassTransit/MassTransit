namespace MassTransit.TestFramework.Indicators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Util;


    /// <summary>
    /// An activity indicator for receive endpoint queues. Utilizes a timer that restarts on receive activity.
    /// </summary>
    public class BusActivityReceiveIndicator : BaseBusActivityIndicatorConnectable, ISignalResource, IReceiveObserver
    {
        readonly ISignalResource _signalResource;
        readonly RollingTimer _receiveIdleTimer;
        int _activityStarted = 0;

        public BusActivityReceiveIndicator(ISignalResource signalResource, TimeSpan receiveIdleTimeout)
        {
            _signalResource = signalResource;
            _receiveIdleTimer = new RollingTimer(SignalInactivity, receiveIdleTimeout);
        }
        
        public BusActivityReceiveIndicator(ISignalResource signalResource) :
            this(signalResource, TimeSpan.FromSeconds(5))
        {
        }

        public BusActivityReceiveIndicator(TimeSpan receiveIdleTimeout) :
            this(null, receiveIdleTimeout)
        {
        }
        
        public BusActivityReceiveIndicator() :
            this(null)
        {
        }
        
        Task IReceiveObserver.PreReceive(ReceiveContext context)
        {
            Interlocked.CompareExchange(ref _activityStarted, 1, 0);
            _receiveIdleTimer.Restart();
            return TaskUtil.Completed;
        }

        Task IReceiveObserver.PostReceive(ReceiveContext context)
        {
            _receiveIdleTimer.Restart();
            return TaskUtil.Completed;
        }

        Task IReceiveObserver.PostConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
        {
            return TaskUtil.Completed;
        }

        Task IReceiveObserver.ConsumeFault<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
        {
            return TaskUtil.Completed;
        }

        Task IReceiveObserver.ReceiveFault(ReceiveContext context, Exception exception)
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