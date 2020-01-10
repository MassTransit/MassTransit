namespace MassTransit.RedisIntegration.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using Saga;
    using Util;


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

        public Task DisposeAsync(CancellationToken cancellationToken)
        {
            return _isCompleted
                ? TaskUtil.Completed
                : _mode == SagaConsumeContextMode.Add
                    ? _database.Add(this)
                    : _database.Update(this);
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
