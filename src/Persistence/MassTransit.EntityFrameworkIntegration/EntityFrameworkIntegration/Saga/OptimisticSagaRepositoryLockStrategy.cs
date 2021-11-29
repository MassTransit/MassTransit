namespace MassTransit.EntityFrameworkIntegration.Saga
{
    using System;
    using System.Data;
    using System.Data.Entity;
    using System.Threading;
    using System.Threading.Tasks;


    public class OptimisticSagaRepositoryLockStrategy<TSaga> :
        ISagaRepositoryLockStrategy<TSaga>
        where TSaga : class, ISaga
    {
        readonly ILoadQueryExecutor<TSaga> _executor;
        readonly ILoadQueryProvider<TSaga> _provider;

        public OptimisticSagaRepositoryLockStrategy(ILoadQueryProvider<TSaga> provider, ILoadQueryExecutor<TSaga> executor, IsolationLevel isolationLevel)
        {
            _provider = provider;
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
            return new OptimisticSagaLockContext<TSaga>(context, query, cancellationToken, _provider);
        }
    }
}
