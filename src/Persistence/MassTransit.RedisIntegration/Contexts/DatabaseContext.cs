namespace MassTransit.RedisIntegration.Contexts
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;


    public interface DatabaseContext<TSaga> :
        IAsyncDisposable
        where TSaga : class, IVersionedSaga
    {
        Task Add<T>(SagaConsumeContext<TSaga, T> context)
            where T : class;

        Task Insert<T>(TSaga instance)
            where T : class;

        Task<TSaga> Load<T>(Guid correlationId)
            where T : class;

        Task Update<T>(SagaConsumeContext<TSaga, T> context)
            where T : class;

        Task Delete<T>(SagaConsumeContext<TSaga, T> context)
            where T : class;
    }
}
