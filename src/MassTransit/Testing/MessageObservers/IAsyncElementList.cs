namespace MassTransit.Testing.MessageObservers
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;


    public interface IAsyncElementList<out TElement>
        where TElement : class, IAsyncListElement
    {
        IEnumerable<TElement> Select(FilterDelegate<TElement> filter, CancellationToken cancellationToken = default);

        IAsyncEnumerable<TElement> SelectAsync(FilterDelegate<TElement> filter, [EnumeratorCancellation] CancellationToken cancellationToken = default);

        Task<bool> Any(FilterDelegate<TElement> filter, CancellationToken cancellationToken = default);
    }
}
