namespace MassTransit.Azure.Table
{
    using System;
    using System.Threading.Tasks;


    public interface DatabaseContext<TSaga> :
        IAsyncDisposable
        where TSaga : class, IVersionedSaga
    {
        Task Add(SagaConsumeContext<TSaga> context);

        Task Insert(TSaga instance);

        Task<TSaga> Load(Guid correlationId);

        Task Update(SagaConsumeContext<TSaga> context);

        Task Delete(SagaConsumeContext<TSaga> context);
    }
}
