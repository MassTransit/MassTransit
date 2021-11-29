namespace MassTransit.Testing
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;


    public interface IReceivedMessageList :
        IAsyncElementList<IReceivedMessage>
    {
        IEnumerable<IReceivedMessage<T>> Select<T>(CancellationToken cancellationToken = default)
            where T : class;

        IEnumerable<IReceivedMessage<T>> Select<T>(FilterDelegate<IReceivedMessage<T>> filter, CancellationToken cancellationToken = default)
            where T : class;

        IAsyncEnumerable<IReceivedMessage> SelectAsync(Action<ReceivedMessageFilter> apply, CancellationToken cancellationToken = default);

        IAsyncEnumerable<IReceivedMessage<T>> SelectAsync<T>(CancellationToken cancellationToken = default)
            where T : class;

        IAsyncEnumerable<IReceivedMessage<T>> SelectAsync<T>(FilterDelegate<IReceivedMessage<T>> filter, CancellationToken cancellationToken = default)
            where T : class;

        Task<bool> Any(Action<ReceivedMessageFilter> apply = default, CancellationToken cancellationToken = default);

        Task<bool> Any<T>(CancellationToken cancellationToken = default)
            where T : class;

        Task<bool> Any<T>(FilterDelegate<IReceivedMessage<T>> filter, CancellationToken cancellationToken = default)
            where T : class;
    }


    public interface IReceivedMessageList<out T> :
        IAsyncElementList<IReceivedMessage<T>>
        where T : class
    {
        IEnumerable<IReceivedMessage<T>> Select(CancellationToken cancellationToken = default);

        IAsyncEnumerable<IReceivedMessage<T>> SelectAsync(CancellationToken cancellationToken = default);

        Task<bool> Any(CancellationToken cancellationToken = default);
    }
}
