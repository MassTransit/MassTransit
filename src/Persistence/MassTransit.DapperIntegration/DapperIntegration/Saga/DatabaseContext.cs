namespace MassTransit.DapperIntegration.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;


    public interface DatabaseContext<TSaga> :
        IAsyncDisposable
        where TSaga : class, ISaga
    {
        Task DeleteAsync(TSaga instance, CancellationToken cancellationToken);

        Task<TSaga> LoadAsync(Guid correlationId, CancellationToken cancellationToken);

        Task<IEnumerable<TSaga>> QueryAsync(Expression<Func<TSaga, bool>> filterExpression, CancellationToken cancellationToken);

        Task InsertAsync(TSaga instance, CancellationToken cancellationToken = default);

        Task UpdateAsync(TSaga instance, CancellationToken cancellationToken = default);

        void Commit();
    }
}
