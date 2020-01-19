namespace MassTransit.EntityFrameworkCoreIntegration.Saga
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.Saga;
    using Microsoft.EntityFrameworkCore;


    public interface ILoadQueryExecutor<TSaga>
        where TSaga : class, ISaga
    {
        Task<TSaga> Load(DbContext dbContext, Guid correlationId, CancellationToken cancellationToken);
    }
}
