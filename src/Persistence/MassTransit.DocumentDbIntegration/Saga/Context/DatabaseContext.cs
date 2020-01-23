namespace MassTransit.DocumentDbIntegration.Saga.Context
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;


    public interface DatabaseContext<TSaga>
        where TSaga : class, IVersionedSaga
    {
        IDocumentClient Client { get; }
        Uri Collection { get; }
        FeedOptions FeedOptions { get; }
        RequestOptions RequestOptions { get; }

        Task Add(TSaga instance, CancellationToken cancellationToken);
        Task<TSaga> Load(Guid correlationId, CancellationToken cancellationToken);
        Task Update(TSaga instance, CancellationToken cancellationToken);
        Task Delete(TSaga instance, CancellationToken cancellationToken);
        Task<TSaga> Insert(TSaga instance, CancellationToken cancellationToken);
    }
}
