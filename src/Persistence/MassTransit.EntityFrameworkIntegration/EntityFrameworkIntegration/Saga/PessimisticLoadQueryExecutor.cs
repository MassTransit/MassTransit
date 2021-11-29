namespace MassTransit.EntityFrameworkIntegration.Saga
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;


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

        public async Task<TSaga> Load(DbContext dbContext, Guid correlationId, CancellationToken cancellationToken)
        {
            var statement = GetLockStatement(dbContext);

            await dbContext.Database.ExecuteSqlCommandAsync(statement, cancellationToken, correlationId).ConfigureAwait(false);

            IQueryable<TSaga> queryable = dbContext.Set<TSaga>();

            if (_queryCustomization != null)
                queryable = _queryCustomization(queryable);

            return await queryable.SingleOrDefaultAsync(x => x.CorrelationId == correlationId, cancellationToken).ConfigureAwait(false);
        }

        string GetLockStatement(DbContext dbContext)
        {
            return _lockStatement ??= _lockStatementProvider.GetRowLockStatement<TSaga>(dbContext);
        }
    }
}
