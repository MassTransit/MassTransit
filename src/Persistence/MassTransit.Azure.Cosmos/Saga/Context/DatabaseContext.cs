namespace MassTransit.Azure.Cosmos.Saga.Context
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos;


    public interface DatabaseContext<TSaga>
        where TSaga : class, IVersionedSaga
    {
        Container Container { get; }
        Action<ItemRequestOptions> ItemRequestOptions { get; }
        Action<QueryRequestOptions> QueryRequestOptions { get; }

        Task Add(TSaga instance, CancellationToken cancellationToken = default);
        Task<TSaga> Load(Guid correlationId, CancellationToken cancellationToken);
        Task Update(TSaga instance, CancellationToken cancellationToken = default);
        Task Delete(TSaga instance, CancellationToken cancellationToken);
        Task<TSaga> Insert(TSaga instance, CancellationToken cancellationToken);
    }
}
