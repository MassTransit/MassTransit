namespace MassTransit.EntityFrameworkIntegration.Saga.Context
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using MassTransit.Context;
    using MassTransit.Saga;


    public class EntityFrameworkSagaConsumeContext<TSaga, TMessage> :
        ConsumeContextScope<TMessage>,
        SagaConsumeContext<TSaga, TMessage>,
        IAsyncDisposable
        where TMessage : class
        where TSaga : class, ISaga
    {
        readonly DbContext _dbContext;
        readonly SagaConsumeContextMode _mode;
        bool _isCompleted;

        public EntityFrameworkSagaConsumeContext(DbContext dbContext, ConsumeContext<TMessage> context, TSaga instance, SagaConsumeContextMode mode)
            : base(context)
        {
            _dbContext = dbContext;
            _mode = mode;

            Saga = instance;
        }

        public async ValueTask DisposeAsync()
        {
            Task Add()
            {
                _dbContext.Set<TSaga>().Add(Saga);

                return _dbContext.SaveChangesAsync();
            }

            if (!_isCompleted)
            {
                if (_mode == SagaConsumeContextMode.Add)
                    await Add().ConfigureAwait(false);
                else
                    await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        Guid? MessageContext.CorrelationId => Saga.CorrelationId;

        bool SagaConsumeContext<TSaga>.IsCompleted => _isCompleted;

        public async Task SetCompleted()
        {
            if (_mode == SagaConsumeContextMode.Insert || _mode == SagaConsumeContextMode.Load)
            {
                _dbContext.Set<TSaga>().Remove(Saga);

                await _dbContext.SaveChangesAsync(CancellationToken).ConfigureAwait(false);
            }

            _isCompleted = true;

            this.LogRemoved();
        }

        public TSaga Saga { get; }
    }
}
