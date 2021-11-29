namespace MassTransit.Testing
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;


    public interface ISagaList<out T> :
        IAsyncElementList<ISagaInstance<T>>
        where T : class, ISaga
    {
        IEnumerable<ISagaInstance<T>> Select(FilterDelegate<T> filter, CancellationToken cancellationToken = default);

        T Contains(Guid sagaId);

        IAsyncEnumerable<ISagaInstance<T>> SelectAsync(CancellationToken cancellationToken = default);

        IAsyncEnumerable<ISagaInstance<T>> SelectAsync(FilterDelegate<T> filter, CancellationToken cancellationToken = default);

        Task<bool> Any(CancellationToken cancellationToken = default);

        Task<bool> Any(FilterDelegate<T> filter, CancellationToken cancellationToken = default);
    }
}
