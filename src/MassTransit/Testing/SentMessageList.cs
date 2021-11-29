namespace MassTransit.Testing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Implementations;


    public class SentMessageList :
        AsyncElementList<ISentMessage>,
        ISentMessageList
    {
        public SentMessageList(TimeSpan timeout, CancellationToken testCompleted = default)
            : base(timeout, testCompleted)
        {
        }

        public IEnumerable<ISentMessage<T>> Select<T>(CancellationToken cancellationToken = default)
            where T : class
        {
            return Select(x => x is ISentMessage<T>, cancellationToken).Cast<ISentMessage<T>>();
        }

        public IEnumerable<ISentMessage<T>> Select<T>(FilterDelegate<ISentMessage<T>> filter, CancellationToken cancellationToken = default)
            where T : class
        {
            var messageFilter = new SentMessageFilter();
            messageFilter.Includes.Add(filter);

            return Select(message => messageFilter.Any(message), cancellationToken).Cast<ISentMessage<T>>();
        }

        public IAsyncEnumerable<ISentMessage> SelectAsync(Action<SentMessageFilter> apply, CancellationToken cancellationToken = default)
        {
            var messageFilter = new SentMessageFilter();
            apply?.Invoke(messageFilter);

            return SelectAsync(message => messageFilter.Any(message), cancellationToken);
        }

        public IAsyncEnumerable<ISentMessage<T>> SelectAsync<T>(CancellationToken cancellationToken = default)
            where T : class
        {
            var messageFilter = new SentMessageFilter();
            messageFilter.Includes.Add<T>();

            return SelectAsync(message => messageFilter.Any(message), cancellationToken).Select<ISentMessage, ISentMessage<T>>();
        }

        public IAsyncEnumerable<ISentMessage<T>> SelectAsync<T>(FilterDelegate<ISentMessage<T>> filter, CancellationToken cancellationToken = default)
            where T : class
        {
            var messageFilter = new SentMessageFilter();
            messageFilter.Includes.Add(filter);

            return SelectAsync(message => messageFilter.Any(message), cancellationToken).Select<ISentMessage, ISentMessage<T>>();
        }

        public Task<bool> Any(Action<SentMessageFilter> apply = default, CancellationToken cancellationToken = default)
        {
            var messageFilter = new SentMessageFilter();
            apply?.Invoke(messageFilter);

            return Any(message => messageFilter.Any(message), cancellationToken);
        }

        public Task<bool> Any<T>(CancellationToken cancellationToken = default)
            where T : class
        {
            var messageFilter = new SentMessageFilter();
            messageFilter.Includes.Add<T>();

            return Any(message => messageFilter.Any(message), cancellationToken);
        }

        public Task<bool> Any<T>(FilterDelegate<ISentMessage<T>> filter, CancellationToken cancellationToken = default)
            where T : class
        {
            var messageFilter = new SentMessageFilter();
            messageFilter.Includes.Add(filter);

            return Any(message => messageFilter.Any(message), cancellationToken);
        }

        public void Add<T>(SendContext<T> context)
            where T : class
        {
            Add(new SentMessage<T>(context));
        }

        public void Add<T>(SendContext<T> context, Exception exception)
            where T : class
        {
            Add(new SentMessage<T>(context, exception));
        }
    }
}
