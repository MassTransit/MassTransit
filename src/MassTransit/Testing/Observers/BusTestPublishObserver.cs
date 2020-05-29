namespace MassTransit.Testing.Observers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MessageObservers;
    using Util;


    public class BusTestPublishObserver :
        IPublishObserver
    {
        readonly PublishedMessageList _messages;

        public BusTestPublishObserver(TimeSpan timeout, CancellationToken testCompleted = default)
        {
            _messages = new PublishedMessageList(timeout, testCompleted);
        }

        public IPublishedMessageList Messages => _messages;

        Task IPublishObserver.PrePublish<T>(PublishContext<T> context)
        {
            return TaskUtil.Completed;
        }

        Task IPublishObserver.PostPublish<T>(PublishContext<T> context)
        {
            _messages.Add(context);

            return TaskUtil.Completed;
        }

        Task IPublishObserver.PublishFault<T>(PublishContext<T> context, Exception exception)
        {
            _messages.Add(context, exception);

            return TaskUtil.Completed;
        }
    }
}
