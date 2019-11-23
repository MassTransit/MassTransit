namespace MassTransit.Testing.MessageObservers
{
    using System;
    using System.Collections.Generic;


    public class ReceivedMessageList :
        MessageList<IReceivedMessage>,
        IReceivedMessageList
    {
        public ReceivedMessageList(TimeSpan timeout)
            : base((int)timeout.TotalMilliseconds)
        {
        }

        public IEnumerable<IReceivedMessage<T>> Select<T>()
            where T : class
        {
            return base.Select<IReceivedMessage<T>>(All);
        }

        public IEnumerable<IReceivedMessage<T>> Select<T>(Func<IReceivedMessage<T>, bool> filter)
            where T : class
        {
            return base.Select(filter);
        }

        public void Add<T>(ConsumeContext<T> context)
            where T : class
        {
            Add(new ReceivedMessage<T>(context), context.MessageId);
        }

        public void Add<T>(ConsumeContext<T> context, Exception exception)
            where T : class
        {
            Add(new ReceivedMessage<T>(context, exception), context.MessageId);
        }

        static bool All<T>(IReceivedMessage<T> _)
            where T : class
        {
            return true;
        }
    }


    public class ReceivedMessageList<T> :
        MessageList<IReceivedMessage<T>>,
        IReceivedMessageList<T>
        where T : class
    {
        public ReceivedMessageList(TimeSpan timeout)
            : base((int)timeout.TotalMilliseconds)
        {
        }

        public void Add(ConsumeContext<T> context)
        {
            Add(new ReceivedMessage<T>(context), context.MessageId);
        }

        public void Add(ConsumeContext<T> context, Exception exception)
        {
            Add(new ReceivedMessage<T>(context, exception), context.MessageId);
        }
    }
}
