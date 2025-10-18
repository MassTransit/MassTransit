namespace MassTransit.EntityFrameworkCoreIntegration
{
    using Microsoft.EntityFrameworkCore;
    using Saga;
    using System;
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;


    public interface ISagaRepositoryLockStrategy<TSaga>
        where TSaga : class, ISaga
    {
        IsolationLevel IsolationLevel { get; }

        Task<TSaga> Load(DbContext context, Guid correlationId, CancellationToken cancellationToken);

        Task<SagaLockContext<TSaga>> CreateLockContext(DbContext context, ISagaQuery<TSaga> query, CancellationToken cancellationToken);

        bool IsTransactionEnabled { get; }
    }
}
