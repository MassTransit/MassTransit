namespace MassTransit.Testing.Observers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MessageObservers;


    /// <summary>
    /// Observes sent messages for test fixtures
    /// </summary>
    public class BusTestSendObserver :
        ISendObserver
    {
        readonly SentMessageList _messages;

        public BusTestSendObserver(TimeSpan timeout, CancellationToken testCompleted = default)
        {
            _messages = new SentMessageList(timeout, testCompleted);
        }

        public ISentMessageList Messages => _messages;

        public async Task PreSend<T>(SendContext<T> context)
            where T : class
        {
        }

        public async Task PostSend<T>(SendContext<T> context)
            where T : class
        {
            _messages.Add(context);
        }

        public async Task SendFault<T>(SendContext<T> context, Exception exception)
            where T : class
        {
            _messages.Add(context, exception);
        }
    }
}
