namespace MassTransit.EntityFrameworkIntegration.Saga
{
    using System;
    using System.Data.Entity;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.Saga;


    public interface ILoadQueryExecutor<TSaga>
        where TSaga : class, ISaga
    {
        Task<TSaga> Load(DbContext dbContext, Guid correlationId, CancellationToken cancellationToken);
    }
}
