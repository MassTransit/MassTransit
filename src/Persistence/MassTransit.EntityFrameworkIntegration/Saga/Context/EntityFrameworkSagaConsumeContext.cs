namespace MassTransit.EntityFrameworkIntegration.Saga.Context
{
    using System;
    using System.Data.Entity;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.Context;
    using MassTransit.Saga;
    using Util;


    public class EntityFrameworkSagaConsumeContext<TSaga, TMessage> :
        ConsumeContextScope<TMessage>,
        SagaConsumeContext<TSaga, TMessage>,
        GreenPipes.IAsyncDisposable
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

        public Task DisposeAsync(CancellationToken cancellationToken)
        {
            Task Add()
            {
                _dbContext.Set<TSaga>().Add(Saga);

                return _dbContext.SaveChangesAsync(cancellationToken);
            }

            return _isCompleted
                ? TaskUtil.Completed
                : _mode == SagaConsumeContextMode.Add
                    ? Add()
                    : _dbContext.SaveChangesAsync(cancellationToken);
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
