namespace MassTransit.Testing.Implementations
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public class BusActivityConsumeIndicator : BaseBusActivityIndicatorConnectable,
        ISignalResource,
        IConsumeObserver
    {
        readonly ISignalResource _signalResource;
        int _messagesInFlight;

        public BusActivityConsumeIndicator(ISignalResource signalResource)
        {
            _signalResource = signalResource;
        }

        public BusActivityConsumeIndicator()
            :
            this(null)
        {
        }

        public override bool IsMet => Interlocked.CompareExchange(ref _messagesInFlight, int.MinValue, int.MinValue) == 0;

        Task IConsumeObserver.PreConsume<T>(ConsumeContext<T> context)
        {
            Interlocked.Increment(ref _messagesInFlight);
            return Task.CompletedTask;
        }

        Task IConsumeObserver.PostConsume<T>(ConsumeContext<T> context)
        {
            if (Interlocked.Decrement(ref _messagesInFlight) == 0)
                Signal();
            return Task.CompletedTask;
        }

        Task IConsumeObserver.ConsumeFault<T>(ConsumeContext<T> context, Exception exception)
        {
            if (Interlocked.Decrement(ref _messagesInFlight) == 0)
                Signal();
            return Task.CompletedTask;
        }

        public void Signal()
        {
            _signalResource?.Signal();
            ConditionUpdated();
        }
    }
}
