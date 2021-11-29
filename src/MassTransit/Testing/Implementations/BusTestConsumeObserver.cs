namespace MassTransit.Testing.Implementations
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public class BusTestConsumeObserver :
        InactivityTestObserver,
        IConsumeObserver
    {
        readonly ReceivedMessageList _messages;
        int _activeCount;

        public BusTestConsumeObserver(TimeSpan timeout, CancellationToken testCompleted)
        {
            _messages = new ReceivedMessageList(timeout, testCompleted);
        }

        public IReceivedMessageList Messages => _messages;

        public override bool IsInactive => Interlocked.CompareExchange(ref _activeCount, int.MinValue, int.MinValue) == 0;

        public Task PreConsume<T>(ConsumeContext<T> context)
            where T : class
        {
            Interlocked.Increment(ref _activeCount);

            return Task.CompletedTask;
        }

        public Task PostConsume<T>(ConsumeContext<T> context)
            where T : class
        {
            _messages.Add(context);

            if (Interlocked.Decrement(ref _activeCount) == 0)
                NotifyInactive();

            return Task.CompletedTask;
        }

        public Task ConsumeFault<T>(ConsumeContext<T> context, Exception exception)
            where T : class
        {
            _messages.Add(context, exception);

            if (Interlocked.Decrement(ref _activeCount) == 0)
                NotifyInactive();

            return Task.CompletedTask;
        }
    }
}
