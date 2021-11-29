namespace MassTransit.DapperIntegration.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;


    public interface DatabaseContext<TSaga>
    {
        Task DeleteAsync<T>(T instance, CancellationToken cancellationToken)
            where T : class, ISaga;

        Task<T> LoadAsync<T>(Guid correlationId, CancellationToken cancellationToken)
            where T : class, ISaga;

        Task<IEnumerable<T>> QueryAsync<T>(Expression<Func<T, bool>> filterExpression, CancellationToken cancellationToken)
            where T : class, ISaga;

        Task InsertAsync<T>(T instance, CancellationToken cancellationToken = default)
            where T : class, ISaga;

        Task UpdateAsync<T>(T instance, CancellationToken cancellationToken = default)
            where T : class, ISaga;
    }
}
