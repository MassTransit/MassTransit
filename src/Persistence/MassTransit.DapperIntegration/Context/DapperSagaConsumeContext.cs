namespace MassTransit.DapperIntegration.Context
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Context;
    using Saga;


    public class DapperSagaConsumeContext<TSaga, TMessage> :
        ConsumeContextScope<TMessage>,
        SagaConsumeContext<TSaga, TMessage>,
        IAsyncDisposable
        where TMessage : class
        where TSaga : class, ISaga
    {
        readonly DatabaseContext<TSaga> _context;
        readonly SagaConsumeContextMode _mode;
        bool _isCompleted;

        public DapperSagaConsumeContext(DatabaseContext<TSaga> context, ConsumeContext<TMessage> consumeContext, TSaga instance, SagaConsumeContextMode mode)
            : base(consumeContext)
        {
            _context = context;
            _mode = mode;

            Saga = instance;
        }

        public async ValueTask DisposeAsync()
        {
            if (!_isCompleted)
            {
                if (_mode == SagaConsumeContextMode.Add)
                    await _context.InsertAsync(Saga).ConfigureAwait(false);
                else
                    await _context.UpdateAsync(Saga).ConfigureAwait(false);
            }
        }

        public TSaga Saga { get; }

        bool SagaConsumeContext<TSaga>.IsCompleted => _isCompleted;

        public async Task SetCompleted()
        {
            if (_mode == SagaConsumeContextMode.Insert || _mode == SagaConsumeContextMode.Load)
            {
                await _context.DeleteAsync(Saga, CancellationToken).ConfigureAwait(false);

                this.LogRemoved();
            }

            _isCompleted = true;
        }
    }
}
