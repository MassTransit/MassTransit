namespace MassTransit.TestFramework.Indicators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Util;


    /// <summary>
    /// An activity indicator for receive endpoint queues. Utilizes a timer that restarts on receive activity.
    /// </summary>
    public class BusActivityReceiveIndicator : ISignalResource, IObservableCondition, IReceiveObserver
    {
        readonly ISignalResource _signalResource;
        readonly RollingTimer _receiveIdleTimer;
        readonly List<IConditionObserver> _observers = new List<IConditionObserver>(); 

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
            _receiveIdleTimer.Restart();
            ConditionUpdated();
            return TaskUtil.Completed;
        }

        Task IReceiveObserver.PostReceive(ReceiveContext context)
        {
            _receiveIdleTimer.Restart();
            ConditionUpdated();
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
            ConditionUpdated();
            return TaskUtil.Completed;
        }

        void SignalInactivity(object state)
        {
            _signalResource?.Signal();
            ConditionUpdated();
            _receiveIdleTimer.Stop();
        }

        public void Signal()
        {
            SignalInactivity(null);
        }


        public bool State => _receiveIdleTimer.Triggered;
        
        public void RegisterObserver(IConditionObserver observer)
        {
            _observers.Add(observer);
        }

        void ConditionUpdated()
        {
            if (_observers.Any())
                _observers.ForEach(x => x.ConditionUpdated());
        }
        
    }
}