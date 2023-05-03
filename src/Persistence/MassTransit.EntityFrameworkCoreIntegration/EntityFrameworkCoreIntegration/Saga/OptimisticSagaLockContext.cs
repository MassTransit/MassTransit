namespace MassTransit.EntityFrameworkCoreIntegration.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;


    /// <summary>
    /// Defers loading the sagas until the transaction is started
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    public class OptimisticSagaLockContext<TSaga> :
        SagaLockContext<TSaga>
        where TSaga : class, ISaga
    {
        readonly CancellationToken _cancellationToken;
        readonly DbContext _context;
        readonly ISagaQuery<TSaga> _query;
        readonly Func<IQueryable<TSaga>, IQueryable<TSaga>> _queryCustomization;

        public OptimisticSagaLockContext(DbContext context, ISagaQuery<TSaga> query, CancellationToken cancellationToken,
            Func<IQueryable<TSaga>, IQueryable<TSaga>> queryCustomization)
        {
            _context = context;
            _query = query;
            _cancellationToken = cancellationToken;
            _queryCustomization = queryCustomization;
        }

        public async Task<IList<TSaga>> Load()
        {
            IQueryable<TSaga> queryable = _context.Set<TSaga>();
            if (_queryCustomization != null)
                queryable = _queryCustomization(queryable);

            List<TSaga> instances = await queryable.AsTracking()
                .Where(_query.FilterExpression)
                .ToListAsync(_cancellationToken)
                .ConfigureAwait(false);

            return instances;
        }
    }
}
