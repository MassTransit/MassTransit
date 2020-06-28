namespace MassTransit.RedisIntegration.Contexts
{
    using System;
    using System.Threading.Tasks;
    using Saga;


    public interface DatabaseContext<TSaga> :
        IAsyncDisposable
        where TSaga : class, ISagaVersion
    {
        Task Add(SagaConsumeContext<TSaga> context);

        Task Insert(TSaga instance);

        Task<TSaga> Load(Guid correlationId);

        Task Update(SagaConsumeContext<TSaga> context);

        Task Delete(SagaConsumeContext<TSaga> context);
    }
}
