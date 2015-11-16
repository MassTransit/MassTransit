namespace MassTransit.TestFramework.Indicators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Pipeline;
    using Util;


    /// <summary>
    /// An activity indicator for publish endpoints. Utilizes a timer that restarts on publish activity.
    /// </summary>
    public class BusActivityPublishIndicator : ISignalResource, IObservableCondition, IPublishObserver
    {
        readonly ISignalResource _signalResource;
        readonly RollingTimer _receiveIdleTimer;
        readonly List<IConditionObserver> _observers = new List<IConditionObserver>(); 

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
            _receiveIdleTimer.Restart();
            ConditionUpdated();
            return TaskUtil.Completed;
        }

        Task IPublishObserver.PostPublish<T>(PublishContext<T> context)
        {
            _receiveIdleTimer.Restart();
            ConditionUpdated();
            return TaskUtil.Completed;
        }

        Task IPublishObserver.PublishFault<T>(PublishContext<T> context, Exception exception)
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