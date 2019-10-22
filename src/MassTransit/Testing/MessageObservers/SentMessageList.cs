namespace MassTransit.Testing.MessageObservers
{
    using System;
    using System.Collections.Generic;


    public class SentMessageList :
        MessageList<ISentMessage>,
        ISentMessageList
    {
        public SentMessageList(TimeSpan timeout)
            : base((int)timeout.TotalMilliseconds)
        {
        }

        public IEnumerable<ISentMessage<T>> Select<T>()
            where T : class
        {
            return base.Select<ISentMessage<T>>(All);
        }

        public IEnumerable<ISentMessage<T>> Select<T>(Func<ISentMessage<T>, bool> filter)
            where T : class
        {
            return base.Select(filter);
        }

        static bool All<T>(ISentMessage<T> _)
            where T : class
        {
            return true;
        }

        public void Add<T>(SendContext<T> context)
            where T : class
        {
            Add(new SentMessage<T>(context), context.MessageId);
        }

        public void Add<T>(SendContext<T> context, Exception exception)
            where T : class
        {
            Add(new SentMessage<T>(context, exception), context.MessageId);
        }
    }
}
