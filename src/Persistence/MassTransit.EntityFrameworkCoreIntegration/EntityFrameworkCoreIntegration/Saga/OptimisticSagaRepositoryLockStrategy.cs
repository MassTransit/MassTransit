namespace MassTransit.EntityFrameworkCoreIntegration.Saga
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;


    public class OptimisticSagaRepositoryLockStrategy<TSaga> :
        ISagaRepositoryLockStrategy<TSaga>
        where TSaga : class, ISaga
    {
        readonly ILoadQueryExecutor<TSaga> _executor;
        readonly Func<IQueryable<TSaga>, IQueryable<TSaga>> _queryCustomization;

        public OptimisticSagaRepositoryLockStrategy(ILoadQueryExecutor<TSaga> executor, Func<IQueryable<TSaga>, IQueryable<TSaga>> queryCustomization,
            IsolationLevel isolationLevel)
        {
            _executor = executor;
            _queryCustomization = queryCustomization;

            IsolationLevel = isolationLevel;
        }

        public IsolationLevel IsolationLevel { get; }

        public Task<TSaga> Load(DbContext context, Guid correlationId, CancellationToken cancellationToken)
        {
            return _executor.Load(context, correlationId, cancellationToken);
        }

        public async Task<SagaLockContext<TSaga>> CreateLockContext(DbContext context, ISagaQuery<TSaga> query, CancellationToken cancellationToken)
        {
            return new OptimisticSagaLockContext<TSaga>(context, query, cancellationToken, _queryCustomization);
        }
    }
}
