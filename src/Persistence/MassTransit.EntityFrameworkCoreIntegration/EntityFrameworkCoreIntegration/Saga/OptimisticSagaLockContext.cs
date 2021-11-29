namespace MassTransit.EntityFrameworkCoreIntegration.Saga
{
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
        readonly ILoadQueryProvider<TSaga> _provider;
        readonly ISagaQuery<TSaga> _query;

        public OptimisticSagaLockContext(DbContext context, ISagaQuery<TSaga> query, CancellationToken cancellationToken, ILoadQueryProvider<TSaga> provider)
        {
            _context = context;
            _query = query;
            _cancellationToken = cancellationToken;
            _provider = provider;
        }

        public async Task<IList<TSaga>> Load()
        {
            List<TSaga> instances = await _provider.GetQueryable(_context)
                .Where(_query.FilterExpression)
                .ToListAsync(_cancellationToken)
                .ConfigureAwait(false);

            return instances;
        }
    }
}
