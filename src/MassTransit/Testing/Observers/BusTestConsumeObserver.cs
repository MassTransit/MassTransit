namespace MassTransit.Testing.Observers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MessageObservers;
    using Util;


    public class BusTestConsumeObserver :
        IConsumeObserver
    {
        readonly ReceivedMessageList _messages;

        public BusTestConsumeObserver(TimeSpan timeout, CancellationToken testCompleted)
        {
            _messages = new ReceivedMessageList(timeout, testCompleted);
        }

        public IReceivedMessageList Messages => _messages;

        public Task PreConsume<T>(ConsumeContext<T> context)
            where T : class
        {
            return TaskUtil.Completed;
        }

        public Task PostConsume<T>(ConsumeContext<T> context)
            where T : class
        {
            _messages.Add(context);

            return TaskUtil.Completed;
        }

        public Task ConsumeFault<T>(ConsumeContext<T> context, Exception exception)
            where T : class
        {
            _messages.Add(context, exception);

            return TaskUtil.Completed;
        }
    }
}
