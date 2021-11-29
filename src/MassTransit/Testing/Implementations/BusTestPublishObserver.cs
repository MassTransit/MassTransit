namespace MassTransit.Testing.Implementations
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public class BusTestPublishObserver :
        InactivityTestObserver,
        IPublishObserver
    {
        readonly PublishedMessageList _messages;

        public BusTestPublishObserver(TimeSpan timeout, TimeSpan inactivityTimout, CancellationToken testCompleted = default)
        {
            _messages = new PublishedMessageList(timeout, testCompleted);

            StartTimer(inactivityTimout);
        }

        public IPublishedMessageList Messages => _messages;

        Task IPublishObserver.PrePublish<T>(PublishContext<T> context)
        {
            return RestartTimer();
        }

        Task IPublishObserver.PostPublish<T>(PublishContext<T> context)
        {
            _messages.Add(context);

            return Task.CompletedTask;
        }

        Task IPublishObserver.PublishFault<T>(PublishContext<T> context, Exception exception)
        {
            _messages.Add(context, exception);

            return Task.CompletedTask;
        }
    }
}
