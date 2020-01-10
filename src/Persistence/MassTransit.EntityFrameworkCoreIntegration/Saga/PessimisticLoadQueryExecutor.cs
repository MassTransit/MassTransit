namespace MassTransit.EntityFrameworkCoreIntegration.Saga
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.Saga;
    using Microsoft.EntityFrameworkCore;


    public class PessimisticLoadQueryExecutor<TSaga> :
        ILoadQueryExecutor<TSaga>
        where TSaga : class, ISaga
    {
        readonly ILoadQueryProvider<TSaga> _queryProvider;
        readonly ILockStatementProvider _lockStatementProvider;
        string _lockStatement;

        public PessimisticLoadQueryExecutor(ILoadQueryProvider<TSaga> queryProvider, ILockStatementProvider lockStatementProvider)
        {
            _queryProvider = queryProvider;
            _lockStatementProvider = lockStatementProvider;
        }

        public Task<TSaga> Load(DbContext dbContext, Guid correlationId, CancellationToken cancellationToken)
        {
            var queryable = _queryProvider.GetQueryable(dbContext);

            var statement = GetLockStatement(dbContext);

            return queryable.FromSql(statement, correlationId).SingleOrDefaultAsync(cancellationToken);
        }

        string GetLockStatement(DbContext dbContext)
        {
            return _lockStatement ??= _lockStatementProvider.GetRowLockStatement<TSaga>(dbContext);
        }
    }
}
