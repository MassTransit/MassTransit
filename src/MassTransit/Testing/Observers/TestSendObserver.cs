namespace MassTransit.Testing.Observers
{
    using System;
    using System.Threading.Tasks;
    using MessageObservers;


    /// <summary>
    /// Observes sent messages for test fixtures
    /// </summary>
    public class TestSendObserver :
        ISendObserver
    {
        readonly SentMessageList _messages;

        public TestSendObserver(TimeSpan timeout)
        {
            _messages = new SentMessageList(timeout);
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
