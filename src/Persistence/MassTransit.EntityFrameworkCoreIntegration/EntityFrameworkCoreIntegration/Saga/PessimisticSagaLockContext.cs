namespace MassTransit.EntityFrameworkCoreIntegration.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;


    /// <summary>
    /// Queries the list of saga ids prior to the transaction, and then loads/locks them individually
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    public class PessimisticSagaLockContext<TSaga> :
        SagaLockContext<TSaga>
        where TSaga : class, ISaga
    {
        readonly CancellationToken _cancellationToken;
        readonly DbContext _context;
        readonly ILoadQueryExecutor<TSaga> _executor;
        readonly IList<Guid> _instances;

        public PessimisticSagaLockContext(DbContext context, CancellationToken cancellationToken, IList<Guid> instances, ILoadQueryExecutor<TSaga> executor)
        {
            _context = context;
            _cancellationToken = cancellationToken;
            _instances = instances;
            _executor = executor;
        }

        public async Task<IList<TSaga>> Load()
        {
            var loaded = new List<TSaga>();

            foreach (var correlationId in _instances)
            {
                var result = await _executor.Load(_context, correlationId, _cancellationToken).ConfigureAwait(false);
                if (result != null)
                    loaded.Add(result);
            }

            return loaded;
        }
    }
}
