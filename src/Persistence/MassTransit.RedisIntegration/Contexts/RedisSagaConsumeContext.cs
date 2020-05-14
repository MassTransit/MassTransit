namespace MassTransit.RedisIntegration.Contexts
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using Saga;


    public class RedisSagaConsumeContext<TSaga, TMessage> :
        ConsumeContextScope<TMessage>,
        SagaConsumeContext<TSaga, TMessage>,
        IAsyncDisposable
        where TMessage : class
        where TSaga : class, IVersionedSaga
    {
        readonly DatabaseContext<TSaga> _database;
        readonly SagaConsumeContextMode _mode;
        bool _isCompleted;

        public RedisSagaConsumeContext(DatabaseContext<TSaga> database, ConsumeContext<TMessage> context, TSaga instance, SagaConsumeContextMode mode)
            : base(context)
        {
            _database = database;
            _mode = mode;

            Saga = instance;
        }

        public async ValueTask DisposeAsync()
        {
            if (!_isCompleted)
            {
                if (_mode == SagaConsumeContextMode.Add)
                    await _database.Add(this).ConfigureAwait(false);
                else
                    await _database.Update(this).ConfigureAwait(false);
            }
        }

        Guid? MessageContext.CorrelationId => Saga.CorrelationId;

        bool SagaConsumeContext<TSaga>.IsCompleted => _isCompleted;

        async Task SagaConsumeContext<TSaga>.SetCompleted()
        {
            if (_mode == SagaConsumeContextMode.Insert || _mode == SagaConsumeContextMode.Load)
                await _database.Delete(this).ConfigureAwait(false);

            _isCompleted = true;

            this.LogRemoved();
        }

        public TSaga Saga { get; }
    }
}
