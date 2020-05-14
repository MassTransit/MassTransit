namespace MassTransit.MartenIntegration.Saga.Context
{
    using System;
    using System.Threading.Tasks;
    using Marten;
    using MassTransit.Context;
    using MassTransit.Saga;


    public class MartenSagaConsumeContext<TSaga, TMessage> :
        ConsumeContextScope<TMessage>,
        SagaConsumeContext<TSaga, TMessage>,
        IAsyncDisposable
        where TMessage : class
        where TSaga : class, ISaga
    {
        readonly SagaConsumeContextMode _mode;
        readonly IDocumentSession _session;
        bool _isCompleted;

        public MartenSagaConsumeContext(IDocumentSession session, ConsumeContext<TMessage> context, TSaga instance, SagaConsumeContextMode mode)
            : base(context)
        {
            _session = session;
            _mode = mode;
            Saga = instance;
        }

        public async ValueTask DisposeAsync()
        {
            if (!_isCompleted)
            {
                if (_mode == SagaConsumeContextMode.Add)
                    _session.Store(Saga);
                else
                    await _session.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        Guid? MessageContext.CorrelationId => Saga.CorrelationId;

        bool SagaConsumeContext<TSaga>.IsCompleted => _isCompleted;

        async Task SagaConsumeContext<TSaga>.SetCompleted()
        {
            if (_mode == SagaConsumeContextMode.Insert || _mode == SagaConsumeContextMode.Load)
            {
                _session.Delete(Saga);
                await _session.SaveChangesAsync().ConfigureAwait(false);
            }

            _isCompleted = true;

            this.LogRemoved();
        }

        public TSaga Saga { get; }
    }
}
