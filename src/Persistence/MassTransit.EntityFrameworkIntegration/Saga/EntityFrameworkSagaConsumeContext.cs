namespace MassTransit.EntityFrameworkIntegration.Saga
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using Context;
    using MassTransit.Saga;


    public class EntityFrameworkSagaConsumeContext<TSaga, TMessage> :
        ConsumeContextScope<TMessage>,
        SagaConsumeContext<TSaga, TMessage>
        where TMessage : class
        where TSaga : class, ISaga
    {
        readonly DbContext _dbContext;
        readonly bool _existing;

        public EntityFrameworkSagaConsumeContext(DbContext dbContext, ConsumeContext<TMessage> context, TSaga instance, bool existing = true)
            : base(context)
        {
            Saga = instance;
            _dbContext = dbContext;
            _existing = existing;
        }

        Guid? MessageContext.CorrelationId => Saga.CorrelationId;

        public async Task SetCompleted()
        {
            IsCompleted = true;
            if (_existing)
            {
                _dbContext.Set<TSaga>().Remove(Saga);

                this.LogRemoved();

                await _dbContext.SaveChangesAsync(CancellationToken).ConfigureAwait(false);
            }
        }

        public bool IsCompleted { get; private set; }
        public TSaga Saga { get; }
    }
}
