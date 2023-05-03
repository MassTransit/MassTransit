namespace MassTransit.Internals
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;


    public static class AsyncEnumerableExtensions
    {
        public static async Task<IList<TElement>> ToListAsync<TElement>(this IAsyncEnumerable<TElement> elements)
            where TElement : class
        {
            var elementsList = new List<TElement>();
            await foreach (var element in elements.ConfigureAwait(false))
                elementsList.Add(element);

            return elementsList;
        }

        public static async Task<IList<TElement>> ToListAsync<TElement>(this IAsyncEnumerable<TElement> elements, CancellationToken cancellationToken)
            where TElement : class
        {
            var elementsList = new List<TElement>();
            await foreach (var element in elements.WithCancellation(cancellationToken).ConfigureAwait(false))
                elementsList.Add(element);

            return elementsList;
        }
    }
}
