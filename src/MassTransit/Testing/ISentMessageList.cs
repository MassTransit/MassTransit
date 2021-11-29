namespace MassTransit.Testing
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;


    public interface ISentMessageList :
        IAsyncElementList<ISentMessage>
    {
        IEnumerable<ISentMessage<T>> Select<T>(CancellationToken cancellationToken = default)
            where T : class;

        IEnumerable<ISentMessage<T>> Select<T>(FilterDelegate<ISentMessage<T>> filter, CancellationToken cancellationToken = default)
            where T : class;

        IAsyncEnumerable<ISentMessage> SelectAsync(Action<SentMessageFilter> apply, CancellationToken cancellationToken = default);

        IAsyncEnumerable<ISentMessage<T>> SelectAsync<T>(CancellationToken cancellationToken = default)
            where T : class;

        IAsyncEnumerable<ISentMessage<T>> SelectAsync<T>(FilterDelegate<ISentMessage<T>> filter, CancellationToken cancellationToken = default)
            where T : class;

        Task<bool> Any(Action<SentMessageFilter> apply = default, CancellationToken cancellationToken = default);

        Task<bool> Any<T>(CancellationToken cancellationToken = default)
            where T : class;

        Task<bool> Any<T>(FilterDelegate<ISentMessage<T>> filter, CancellationToken cancellationToken = default)
            where T : class;
    }
}
