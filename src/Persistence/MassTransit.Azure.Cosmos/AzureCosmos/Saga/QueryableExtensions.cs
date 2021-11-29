namespace MassTransit.AzureCosmos.Saga
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Azure.Cosmos.Linq;


    public static class QueryableExtensions
    {
        // Loads all to memory. If the number of saga instances is quite large (shouldn't be), then this could run up memory usage fast
        public static async Task<IEnumerable<T>> QueryAsync<T>(this IQueryable<T> query, CancellationToken cancellationToken)
        {
            FeedIterator<T> feedIterator = query.ToFeedIterator();
            var batches = new List<T>();

            while (feedIterator.HasMoreResults)
                batches.AddRange(await feedIterator.ReadNextAsync(cancellationToken).ConfigureAwait(false));

            return batches;
        }
    }
}
