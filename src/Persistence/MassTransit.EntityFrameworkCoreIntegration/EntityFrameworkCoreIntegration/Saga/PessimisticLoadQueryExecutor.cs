namespace MassTransit.EntityFrameworkCoreIntegration.Saga
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using EntityFrameworkCoreIntegration;
    using Microsoft.EntityFrameworkCore;


    public class PessimisticLoadQueryExecutor<TSaga> :
        ILoadQueryExecutor<TSaga>
        where TSaga : class, ISaga
    {
        readonly ILockStatementProvider _lockStatementProvider;
        readonly Func<IQueryable<TSaga>, IQueryable<TSaga>> _queryCustomization;
        string _lockStatement;

        public PessimisticLoadQueryExecutor(ILockStatementProvider lockStatementProvider, Func<IQueryable<TSaga>, IQueryable<TSaga>> queryCustomization)
        {
            _lockStatementProvider = lockStatementProvider;
            _queryCustomization = queryCustomization;
        }

        public Task<TSaga> Load(DbContext dbContext, Guid correlationId, CancellationToken cancellationToken)
        {
            var statement = GetLockStatement(dbContext);

            IQueryable<TSaga> queryable = dbContext.Set<TSaga>().FromSqlRaw(statement, correlationId);

            if (_queryCustomization != null)
                queryable = _queryCustomization(queryable);

            return queryable.SingleOrDefaultAsync(cancellationToken);
        }

        string GetLockStatement(DbContext dbContext)
        {
            return _lockStatement ??= _lockStatementProvider.GetRowLockStatement<TSaga>(dbContext);
        }
    }
}
