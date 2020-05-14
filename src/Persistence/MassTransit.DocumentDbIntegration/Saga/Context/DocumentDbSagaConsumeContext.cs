namespace MassTransit.DocumentDbIntegration.Saga.Context
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Context;
    using MassTransit.Saga;


    public class DocumentDbSagaConsumeContext<TSaga, TMessage> :
        ConsumeContextScope<TMessage>,
        SagaConsumeContext<TSaga, TMessage>,
        IAsyncDisposable
        where TMessage : class
        where TSaga : class, IVersionedSaga
    {
        readonly DatabaseContext<TSaga> _databaseContext;
        readonly SagaConsumeContextMode _mode;
        bool _isCompleted;

        public DocumentDbSagaConsumeContext(DatabaseContext<TSaga> databaseContext, ConsumeContext<TMessage> context, TSaga instance,
            SagaConsumeContextMode mode)
            : base(context)
        {
            _databaseContext = databaseContext;
            _mode = mode;
            Saga = instance;
        }

        Guid? MessageContext.CorrelationId => Saga.CorrelationId;

        public async Task SetCompleted()
        {
            if (_mode == SagaConsumeContextMode.Insert || _mode == SagaConsumeContextMode.Load)
            {
                await _databaseContext.Delete(Saga, CancellationToken).ConfigureAwait(false);

                this.LogRemoved();
            }

            _isCompleted = true;
        }

        public TSaga Saga { get; }

        public bool IsCompleted
        {
            get => _isCompleted;
            set => _isCompleted = value;
        }

        public async ValueTask DisposeAsync()
        {
            if (!_isCompleted)
            {
                if (_mode == SagaConsumeContextMode.Add)
                    await _databaseContext.Add(Saga).ConfigureAwait(false);
                else
                    await _databaseContext.Update(Saga).ConfigureAwait(false);
            }
        }
    }
}
