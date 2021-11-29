namespace MassTransit.Testing
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;


    public interface IPublishedMessageList :
        IAsyncElementList<IPublishedMessage>
    {
        IEnumerable<IPublishedMessage<T>> Select<T>(CancellationToken cancellationToken = default)
            where T : class;

        IEnumerable<IPublishedMessage<T>> Select<T>(FilterDelegate<IPublishedMessage<T>> filter, CancellationToken cancellationToken = default)
            where T : class;

        IAsyncEnumerable<IPublishedMessage> SelectAsync(Action<PublishedMessageFilter> apply, CancellationToken cancellationToken = default);

        IAsyncEnumerable<IPublishedMessage<T>> SelectAsync<T>(CancellationToken cancellationToken = default)
            where T : class;

        IAsyncEnumerable<IPublishedMessage<T>> SelectAsync<T>(FilterDelegate<IPublishedMessage<T>> filter, CancellationToken cancellationToken = default)
            where T : class;

        Task<bool> Any(Action<PublishedMessageFilter> apply = default, CancellationToken cancellationToken = default);

        Task<bool> Any<T>(CancellationToken cancellationToken = default)
            where T : class;

        Task<bool> Any<T>(FilterDelegate<IPublishedMessage<T>> filter, CancellationToken cancellationToken = default)
            where T : class;
    }
}
