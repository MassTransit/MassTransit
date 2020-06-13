namespace MassTransit.Azure.Cosmos.Saga
{
    using Microsoft.Azure.Cosmos.Linq;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;


    public static class QueryableExtensions
    {
        // Loads all to memory. If the number of saga instances is quite large (shouldn't be), then this could run up memory usage fast
        public static async Task<IEnumerable<T>> QueryAsync<T>(this IQueryable<T> query, CancellationToken cancellationToken)
        {
            var feedIterator = query.ToFeedIterator();
            var batches = new List<T>();

            while(feedIterator.HasMoreResults)
            {
                foreach (var item in await feedIterator.ReadNextAsync(cancellationToken).ConfigureAwait(false))
                {
                    batches.Add(item);
                }
            }

            return batches;
        }
    }
}
