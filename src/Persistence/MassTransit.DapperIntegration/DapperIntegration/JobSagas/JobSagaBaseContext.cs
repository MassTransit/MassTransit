#nullable enable
namespace MassTransit.DapperIntegration.JobSagas
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Saga;

    public abstract class JobSagaBaseContext<TSaga, TModel>
        where TSaga : class, ISaga
        where TModel : class, ISaga
    {
        readonly DatabaseContext<TModel> _databaseContext;
        readonly DapperSagaSerializer<TSaga, TModel> _serializer;

        protected JobSagaBaseContext(DatabaseContext<TModel> databaseContext, DapperSagaSerializer<TSaga, TModel> serializer)
        {
            _databaseContext = databaseContext;
            _serializer = serializer;
        }

        public async Task<TSaga?> LoadAsync(Guid correlationId, CancellationToken cancellationToken = default)
        {
            var model = await _databaseContext.LoadAsync(correlationId, cancellationToken);
            return _serializer.FromModel(model);
        }

        public Task<IEnumerable<TSaga>> QueryAsync(Expression<Func<TSaga, bool>> filterExpression, CancellationToken cancellationToken = default) 
            => throw new NotImplementedByDesignException("Job sagas use different models for persistence and cannot be queried with the Dapper provider");

        public Task InsertAsync(TSaga instance, CancellationToken cancellationToken = default)
        {
            var model = _serializer.FromSaga(instance);
            return _databaseContext.InsertAsync(
                model,
                cancellationToken
            );
        }

        public Task UpdateAsync(TSaga instance, CancellationToken cancellationToken = default)
        {
            var model = _serializer.FromSaga(instance);
            return _databaseContext.UpdateAsync(
                model,
                cancellationToken
            );
        }

        public Task DeleteAsync(TSaga instance, CancellationToken cancellationToken = default)
        {
            var model = _serializer.FromSaga(instance);
            return _databaseContext.DeleteAsync(
                model,
                cancellationToken
            );
        }

        public Task CommitAsync(CancellationToken cancellationToken = default) 
            => _databaseContext.CommitAsync(cancellationToken);

        public ValueTask DisposeAsync() 
            => _databaseContext.DisposeAsync();
    }
}