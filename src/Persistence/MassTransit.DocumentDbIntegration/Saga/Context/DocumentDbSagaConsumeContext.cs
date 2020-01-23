namespace MassTransit.DocumentDbIntegration.Saga.Context
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Context;
    using MassTransit.Saga;
    using Util;


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

        Task IAsyncDisposable.DisposeAsync(CancellationToken cancellationToken)
        {
            return IsCompleted
                ? TaskUtil.Completed
                : _mode == SagaConsumeContextMode.Add
                    ? _databaseContext.Add(Saga, cancellationToken)
                    : _databaseContext.Update(Saga, cancellationToken);
        }
    }
}
