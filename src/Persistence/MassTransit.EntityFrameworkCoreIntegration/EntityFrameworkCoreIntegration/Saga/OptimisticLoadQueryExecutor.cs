namespace MassTransit.EntityFrameworkCoreIntegration.Saga
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;


    public class OptimisticLoadQueryExecutor<TSaga> :
        ILoadQueryExecutor<TSaga>
        where TSaga : class, ISaga
    {
        readonly Func<DbContext, Guid, Task<TSaga>> _compiledQuery;
        readonly Func<IQueryable<TSaga>, IQueryable<TSaga>> _queryCustomization;

        public OptimisticLoadQueryExecutor(Func<IQueryable<TSaga>, IQueryable<TSaga>> queryCustomization = null)
        {
            _queryCustomization = queryCustomization;

            if (queryCustomization == null)
            {
                _compiledQuery = EF.CompileAsyncQuery((DbContext context, Guid id) =>
                    context.Set<TSaga>().AsTracking().SingleOrDefault(x => x.CorrelationId == id));
            }
        }

        public Task<TSaga> Load(DbContext dbContext, Guid correlationId, CancellationToken cancellationToken)
        {
            if (_compiledQuery != null)
                return _compiledQuery(dbContext, correlationId);

            IQueryable<TSaga> queryable = _queryCustomization(dbContext.Set<TSaga>());

            return queryable.AsTracking().SingleOrDefaultAsync(x => x.CorrelationId == correlationId, cancellationToken);
        }
    }
}
