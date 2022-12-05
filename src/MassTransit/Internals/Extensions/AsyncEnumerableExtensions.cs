namespace MassTransit.Internals
{
    using System.Collections.Generic;
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
    }
}
