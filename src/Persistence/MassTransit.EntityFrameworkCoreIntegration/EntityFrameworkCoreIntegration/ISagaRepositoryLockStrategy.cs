namespace MassTransit.EntityFrameworkCoreIntegration
{
    using System;
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Saga;


    public interface ISagaRepositoryLockStrategy<TSaga>
        where TSaga : class, ISaga
    {
        IsolationLevel IsolationLevel { get; }

        Task<TSaga> Load(DbContext context, Guid correlationId, CancellationToken cancellationToken);

        Task<SagaLockContext<TSaga>> CreateLockContext(DbContext context, ISagaQuery<TSaga> query, CancellationToken cancellationToken);
    }
}
