namespace MassTransit.Persistence.Integration.Saga
{
    using System.Linq.Expressions;


    public interface DatabaseContext<TSaga> :
        IAsyncDisposable,
        IDisposable
        where TSaga : class
    {
        Task DeleteAsync(TSaga instance, CancellationToken cancellationToken = default);

        Task<TSaga?> LoadAsync(Guid correlationId, CancellationToken cancellationToken = default);

        IAsyncEnumerable<TSaga> QueryAsync(Expression<Func<TSaga, bool>> filterExpression, CancellationToken cancellationToken = default);

        Task InsertAsync(TSaga instance, CancellationToken cancellationToken = default);

        Task UpdateAsync(TSaga instance, CancellationToken cancellationToken = default);

        Task CommitAsync(CancellationToken cancellationToken = default);
    }
}
