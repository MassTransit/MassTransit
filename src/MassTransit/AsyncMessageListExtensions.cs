namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;


    public static class AsyncMessageListExtensions
    {
        public static async Task<IList<TElement>> ToListAsync<TElement>(this IAsyncEnumerable<TElement> elements)
            where TElement : class
        {
            var elementsList = new List<TElement>();
            await foreach (var element in elements.ConfigureAwait(false))
                elementsList.Add(element);

            return elementsList;
        }

        public static async Task<TElement> First<TElement>(this IAsyncEnumerable<TElement> elements)
            where TElement : class
        {
            await foreach (var element in elements.ConfigureAwait(false))
                return element;

            throw new InvalidOperationException("Message List was empty, or timed out");
        }

        public static async Task<int> Count<TElement>(this IAsyncEnumerable<TElement> elements)
            where TElement : class
        {
            var count = 0;
            await foreach (var element in elements.ConfigureAwait(false))
                count++;

            return count;
        }

        public static async IAsyncEnumerable<TElement> Take<TElement>(this IAsyncEnumerable<TElement> elements, int quantity)
            where TElement : class
        {
            var count = 0;
            await foreach (var element in elements.ConfigureAwait(false))
            {
                yield return element;

                count++;
                if (count == quantity)
                    yield break;
            }
        }

        public static async Task<TElement> FirstOrDefault<TElement>(this IAsyncEnumerable<TElement> elements)
            where TElement : class
        {
            await foreach (var element in elements.ConfigureAwait(false))
                return element;

            return default;
        }

        public static async Task<bool> Any<TElement>(this IAsyncEnumerable<TElement> elements)
            where TElement : class
        {
            try
            {
                await foreach (var _ in elements.ConfigureAwait(false))
                    return true;
            }
            catch (OperationCanceledException)
            {
            }

            return false;
        }

        public static async IAsyncEnumerable<TResult> Select<TElement, TResult>(this IAsyncEnumerable<TElement> elements)
            where TElement : class
            where TResult : class
        {
            await foreach (var entry in elements.ConfigureAwait(false))
            {
                if (entry is TResult result)
                    yield return result;
            }
        }
    }
}
