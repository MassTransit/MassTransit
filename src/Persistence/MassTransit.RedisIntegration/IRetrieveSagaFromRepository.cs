namespace MassTransit.RedisIntegration
{
    using System;
    using System.Threading.Tasks;


    public interface IRetrieveSagaFromRepository<TSaga>
        where TSaga : IVersionedSaga
    {
        Task<TSaga> GetSaga(Guid correlationId);
    }
}
