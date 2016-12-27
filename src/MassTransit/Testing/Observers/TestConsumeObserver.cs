namespace MassTransit.Testing.Observers
{
    using System;
    using System.Threading.Tasks;
    using MessageObservers;
    using Util;


    public class TestConsumeObserver :
        IConsumeObserver
    {
        readonly ReceivedMessageList _messages;

        public TestConsumeObserver(TimeSpan timeout)
        {
            _messages = new ReceivedMessageList(timeout);
        }

        public IReceivedMessageList Messages => _messages;

        Task IConsumeObserver.PreConsume<T>(ConsumeContext<T> context)
        {
            return TaskUtil.Completed;
        }

        Task IConsumeObserver.PostConsume<T>(ConsumeContext<T> context)
        {
            _messages.Add(context);

            return TaskUtil.Completed;
        }

        Task IConsumeObserver.ConsumeFault<T>(ConsumeContext<T> context, Exception exception)
        {
            _messages.Add(context, exception);

            return TaskUtil.Completed;
        }
    }
}