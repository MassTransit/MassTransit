namespace MassTransit.TestFramework.Indicators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Pipeline;
    using Util;


    /// <summary>
    /// An activity indicator for send endpoints. Utilizes a timer that restarts on send activity.
    /// </summary>
    public class BusActivitySendIndicator : ISignalResource, IObservableCondition, ISendObserver
    {
        readonly ISignalResource _signalResource;
        readonly RollingTimer _receiveIdleTimer;
        readonly List<IConditionObserver> _observers = new List<IConditionObserver>(); 

        public BusActivitySendIndicator(ISignalResource signalResource, TimeSpan receiveIdleTimeout)
        {
            _signalResource = signalResource;
            _receiveIdleTimer = new RollingTimer(SignalInactivity, receiveIdleTimeout);
        }
        
        public BusActivitySendIndicator(ISignalResource signalResource) :
            this(signalResource, TimeSpan.FromSeconds(5))
        {
        }

        public BusActivitySendIndicator(TimeSpan receiveIdleTimeout) :
            this(null, receiveIdleTimeout)
        {
        }
        
        public BusActivitySendIndicator() :
            this(null)
        {
        }

        Task ISendObserver.PreSend<T>(SendContext<T> context)
        {
            _receiveIdleTimer.Restart();
            ConditionUpdated();
            return TaskUtil.Completed;
        }

        Task ISendObserver.PostSend<T>(SendContext<T> context)
        {
            _receiveIdleTimer.Restart();
            ConditionUpdated();
            return TaskUtil.Completed;
        }

        Task ISendObserver.SendFault<T>(SendContext<T> context, Exception exception)
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