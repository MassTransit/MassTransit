namespace MassTransit.DapperIntegration.Context
{
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Context;
    using Saga;
    using Util;


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

        public Task DisposeAsync(CancellationToken cancellationToken)
        {
            return _isCompleted
                ? TaskUtil.Completed
                : _mode == SagaConsumeContextMode.Add
                    ? _context.InsertAsync(Saga, cancellationToken)
                    : _context.UpdateAsync(Saga, cancellationToken);
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
