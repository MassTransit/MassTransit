namespace MassTransit.MartenIntegration
{
    using System;
    using System.Threading.Tasks;
    using Saga;


    public interface IRetrieveSagaFromRepository<TSaga>
        where TSaga : ISaga
    {
        Task<TSaga> GetSagaAsync(Guid correlationId);
        TSaga GetSaga(Guid correlationId);
    }
}