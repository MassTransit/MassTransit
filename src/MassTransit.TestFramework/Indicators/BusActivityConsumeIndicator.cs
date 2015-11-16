namespace MassTransit.TestFramework.Indicators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Pipeline;
    using Util;


    public class BusActivityConsumeIndicator : BaseBusActivityIndicatorConnectable, ISignalResource, IConsumeObserver
    {
        readonly ISignalResource _signalResource;
        int _messagesInFlight = 0;
        
        public BusActivityConsumeIndicator(ISignalResource signalResource)
        {
            _signalResource = signalResource;
        }

        public BusActivityConsumeIndicator() :
            this(null)
        {
        }
        

        Task IConsumeObserver.PreConsume<T>(ConsumeContext<T> context)
        {
            if (Interlocked.Increment(ref _messagesInFlight) == 1)
                ConditionUpdated();
            return TaskUtil.Completed;
        }

        Task IConsumeObserver.PostConsume<T>(ConsumeContext<T> context)
        {
            if (Interlocked.Decrement(ref _messagesInFlight) == 0)
                Signal();
            return TaskUtil.Completed;
        }

        Task IConsumeObserver.ConsumeFault<T>(ConsumeContext<T> context, Exception exception)
        {
            if (Interlocked.Decrement(ref _messagesInFlight) == 0)
                Signal();
            return TaskUtil.Completed;
        }

        public void Signal()
        {
            _signalResource?.Signal();
            ConditionUpdated();
        }

        public override bool State => Interlocked.CompareExchange(ref _messagesInFlight, int.MinValue, int.MinValue) == 0;
        
        
    }
}