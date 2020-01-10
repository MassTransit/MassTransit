namespace MassTransit.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;


    public interface IQuerySagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        Task<IEnumerable<Guid>> Find(ISagaQuery<TSaga> query);
    }


    public interface ILoadSagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        Task<TSaga> Load(Guid correlationId);
    }
}
