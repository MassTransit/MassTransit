namespace MassTransit.DocumentDbIntegration
{
    using Microsoft.Azure.Documents.Linq;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public static class QueryableExtensions
    {
        // Loads all to memory. If the number of saga instances is quite large (shouldn't be), then this could run up memory usage fast
        public static async Task<IEnumerable<T>> QueryAsync<T>(this IQueryable<T> query)
        {
            var docQuery = query.AsDocumentQuery();
            var batches = new List<IEnumerable<T>>();

            do
            {
                var batch = await docQuery.ExecuteNextAsync<T>().ConfigureAwait(false);
                batches.Add(batch);
            }
            while (docQuery.HasMoreResults);

            var docs = batches.SelectMany(b => b);

            return docs;
        }
    }
}
