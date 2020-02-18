namespace MassTransit.DocumentDbIntegration.Saga
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.Documents.Linq;


    public static class QueryableExtensions
    {
        // Loads all to memory. If the number of saga instances is quite large (shouldn't be), then this could run up memory usage fast
        public static async Task<IEnumerable<T>> QueryAsync<T>(this IQueryable<T> query, CancellationToken cancellationToken)
        {
            var documentQuery = query.AsDocumentQuery();
            var batches = new List<IEnumerable<T>>();

            do
            {
                var batch = await documentQuery.ExecuteNextAsync<T>(cancellationToken).ConfigureAwait(false);
                batches.Add(batch);
            }
            while (documentQuery.HasMoreResults);

            return batches.SelectMany(b => b);
        }
    }
}
