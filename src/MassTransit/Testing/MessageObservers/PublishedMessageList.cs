namespace MassTransit.Testing.MessageObservers
{
    using System;
    using System.Collections.Generic;


    public class PublishedMessageList :
        MessageList<IPublishedMessage>,
        IPublishedMessageList
    {
        public PublishedMessageList(TimeSpan timeout)
            : base((int)timeout.TotalMilliseconds)
        {
        }

        public IEnumerable<IPublishedMessage<T>> Select<T>()
            where T : class
        {
            return base.Select<IPublishedMessage<T>>(All);
        }

        public IEnumerable<IPublishedMessage<T>> Select<T>(Func<IPublishedMessage<T>, bool> filter)
            where T : class
        {
            return base.Select(filter);
        }

        public void Add<T>(PublishContext<T> context)
            where T : class
        {
            Add(new PublishedMessage<T>(context), context.MessageId);
        }

        public void Add<T>(PublishContext<T> context, Exception exception)
            where T : class
        {
            Add(new PublishedMessage<T>(context, exception), context.MessageId);
        }

        static bool All<T>(IPublishedMessage<T> _)
            where T : class
        {
            return true;
        }
    }
}
