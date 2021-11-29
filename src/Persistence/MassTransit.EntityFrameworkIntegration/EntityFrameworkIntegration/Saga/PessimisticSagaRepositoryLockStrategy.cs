namespace MassTransit.EntityFrameworkIntegration.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;


    public class PessimisticSagaRepositoryLockStrategy<TSaga> :
        ISagaRepositoryLockStrategy<TSaga>
        where TSaga : class, ISaga
    {
        readonly ILoadQueryExecutor<TSaga> _executor;

        public PessimisticSagaRepositoryLockStrategy(ILoadQueryExecutor<TSaga> executor, IsolationLevel isolationLevel)
        {
            _executor = executor;

            IsolationLevel = isolationLevel;
        }

        public IsolationLevel IsolationLevel { get; }

        public Task<TSaga> Load(DbContext context, Guid correlationId, CancellationToken cancellationToken)
        {
            return _executor.Load(context, correlationId, cancellationToken);
        }

        public async Task<SagaLockContext<TSaga>> CreateLockContext(DbContext context, ISagaQuery<TSaga> query, CancellationToken cancellationToken)
        {
            IList<Guid> instances = await context.Set<TSaga>()
                .AsNoTracking()
                .Where(query.FilterExpression)
                .Select(x => x.CorrelationId)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return new PessimisticSagaLockContext<TSaga>(context, cancellationToken, instances, _executor);
        }
    }
}
