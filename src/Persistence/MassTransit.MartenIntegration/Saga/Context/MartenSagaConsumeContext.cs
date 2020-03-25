namespace MassTransit.MartenIntegration.Saga.Context
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Marten;
    using MassTransit.Context;
    using MassTransit.Saga;
    using Util;


    public class MartenSagaConsumeContext<TSaga, TMessage> :
        ConsumeContextScope<TMessage>,
        SagaConsumeContext<TSaga, TMessage>,
        GreenPipes.IAsyncDisposable
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

        public Task DisposeAsync(CancellationToken cancellationToken)
        {
            if (_isCompleted)
                return TaskUtil.Completed;

            if (_mode == SagaConsumeContextMode.Add)
                _session.Store(Saga);

            return _session.SaveChangesAsync(cancellationToken);
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
