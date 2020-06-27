namespace MassTransit.Azure.Table.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Saga;


    public interface DatabaseContext<TSaga>
        where TSaga : class, IVersionedSaga
    {
        Task Add(SagaConsumeContext<TSaga> context);

        Task Insert(TSaga instance, CancellationToken cancellationToken);

        Task<TSaga> Load(Guid correlationId, CancellationToken cancellationToken);

        Task Update(SagaConsumeContext<TSaga> context);

        Task Delete(SagaConsumeContext<TSaga> context);
    }
}
