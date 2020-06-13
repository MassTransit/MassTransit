namespace MassTransit.Azure.Cosmos.Saga.Context
{
    using Microsoft.Azure.Cosmos;
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public interface DatabaseContext<TSaga>
        where TSaga : class, IVersionedSaga
    {
        CosmosClient Client { get; }
        Container Container { get; }
        Action<ItemRequestOptions> ItemRequestOptions { get; }
        Action<QueryRequestOptions> QueryRequestOptions { get; }
        Func<TSaga, PartitionKey> PartitionKeyExpression { get; }

        Task Add(TSaga instance, CancellationToken cancellationToken = default);
        Task<TSaga> Load(Guid correlationId, CancellationToken cancellationToken);
        Task Update(TSaga instance, CancellationToken cancellationToken = default);
        Task Delete(TSaga instance, CancellationToken cancellationToken);
        Task<TSaga> Insert(TSaga instance, CancellationToken cancellationToken);
    }
}
