namespace MassTransit
{
    using System;
    using System.Threading.Tasks;


    public interface ILoadSagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        Task<TSaga> Load(Guid correlationId);
    }
}
