namespace MassTransit.EntityFrameworkIntegration.Saga.Context
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.Saga;


    /// <summary>
    /// Defers loading the sagas until the transaction is started
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    public class OptimisticSagaLockContext<TSaga> :
        SagaLockContext<TSaga>
        where TSaga : class, ISaga
    {
        readonly DbContext _context;
        readonly ISagaQuery<TSaga> _query;
        readonly CancellationToken _cancellationToken;
        readonly ILoadQueryProvider<TSaga> _provider;

        public OptimisticSagaLockContext(DbContext context, ISagaQuery<TSaga> query, CancellationToken cancellationToken, ILoadQueryProvider<TSaga> provider)
        {
            _context = context;
            _query = query;
            _cancellationToken = cancellationToken;
            _provider = provider;
        }

        public async Task<IList<TSaga>> Load()
        {
            var instances = await _provider.GetQueryable(_context)
                .Where(_query.FilterExpression)
                .ToListAsync(_cancellationToken)
                .ConfigureAwait(false);

            return instances;
        }
    }
}
