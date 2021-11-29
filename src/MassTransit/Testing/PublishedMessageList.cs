namespace MassTransit.Testing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Implementations;


    public class PublishedMessageList :
        AsyncElementList<IPublishedMessage>,
        IPublishedMessageList
    {
        public PublishedMessageList(TimeSpan timeout, CancellationToken testCompleted = default)
            : base(timeout, testCompleted)
        {
        }

        public IEnumerable<IPublishedMessage<T>> Select<T>(CancellationToken cancellationToken = default)
            where T : class
        {
            return Select(x => x is IPublishedMessage<T>, cancellationToken).Cast<IPublishedMessage<T>>();
        }

        public IEnumerable<IPublishedMessage<T>> Select<T>(FilterDelegate<IPublishedMessage<T>> filter, CancellationToken cancellationToken = default)
            where T : class
        {
            var messageFilter = new PublishedMessageFilter();
            messageFilter.Includes.Add(filter);

            return Select(message => messageFilter.Any(message), cancellationToken).Cast<IPublishedMessage<T>>();
        }

        public IAsyncEnumerable<IPublishedMessage> SelectAsync(Action<PublishedMessageFilter> apply, CancellationToken cancellationToken = default)
        {
            var messageFilter = new PublishedMessageFilter();
            apply?.Invoke(messageFilter);

            return SelectAsync(message => messageFilter.Any(message), cancellationToken);
        }

        public IAsyncEnumerable<IPublishedMessage<T>> SelectAsync<T>(CancellationToken cancellationToken = default)
            where T : class
        {
            var messageFilter = new PublishedMessageFilter();
            messageFilter.Includes.Add<T>();

            return SelectAsync(message => messageFilter.Any(message), cancellationToken).Select<IPublishedMessage, IPublishedMessage<T>>();
        }

        public IAsyncEnumerable<IPublishedMessage<T>> SelectAsync<T>(FilterDelegate<IPublishedMessage<T>> filter, CancellationToken cancellationToken = default)
            where T : class
        {
            var messageFilter = new PublishedMessageFilter();
            messageFilter.Includes.Add(filter);

            return SelectAsync(message => messageFilter.Any(message), cancellationToken).Select<IPublishedMessage, IPublishedMessage<T>>();
        }

        public Task<bool> Any(Action<PublishedMessageFilter> apply = default, CancellationToken cancellationToken = default)
        {
            var messageFilter = new PublishedMessageFilter();
            apply?.Invoke(messageFilter);

            return Any(message => messageFilter.Any(message), cancellationToken);
        }

        public Task<bool> Any<T>(CancellationToken cancellationToken = default)
            where T : class
        {
            var messageFilter = new PublishedMessageFilter();
            messageFilter.Includes.Add<T>();

            return Any(message => messageFilter.Any(message), cancellationToken);
        }

        public Task<bool> Any<T>(FilterDelegate<IPublishedMessage<T>> filter, CancellationToken cancellationToken = default)
            where T : class
        {
            var messageFilter = new PublishedMessageFilter();
            messageFilter.Includes.Add(filter);

            return Any(message => messageFilter.Any(message), cancellationToken);
        }

        public void Add<T>(PublishContext<T> context)
            where T : class
        {
            Add(new PublishedMessage<T>(context));
        }

        public void Add<T>(PublishContext<T> context, Exception exception)
            where T : class
        {
            Add(new PublishedMessage<T>(context, exception));
        }
    }
}
