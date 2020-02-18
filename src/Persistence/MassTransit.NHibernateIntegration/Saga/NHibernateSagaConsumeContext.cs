namespace MassTransit.NHibernateIntegration.Saga
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using GreenPipes.Util;
    using MassTransit.Saga;
    using NHibernate;


    public class NHibernateSagaConsumeContext<TSaga, TMessage> :
        ConsumeContextScope<TMessage>,
        SagaConsumeContext<TSaga, TMessage>,
        IAsyncDisposable
        where TMessage : class
        where TSaga : class, ISaga
    {
        readonly SagaConsumeContextMode _mode;
        readonly ISession _session;
        bool _isCompleted;

        public NHibernateSagaConsumeContext(ISession session, ConsumeContext<TMessage> context, TSaga instance, SagaConsumeContextMode mode)
            : base(context)
        {
            Saga = instance;
            _session = session;
            _mode = mode;
        }

        public Task DisposeAsync(CancellationToken cancellationToken)
        {
            return _isCompleted
                ? TaskUtil.Completed
                : _session.SaveAsync(Saga, cancellationToken);
        }

        Guid? MessageContext.CorrelationId => Saga.CorrelationId;

        bool SagaConsumeContext<TSaga>.IsCompleted => _isCompleted;

        async Task SagaConsumeContext<TSaga>.SetCompleted()
        {
            if (_mode == SagaConsumeContextMode.Insert || _mode == SagaConsumeContextMode.Load)
                await _session.DeleteAsync(Saga).ConfigureAwait(false);

            _isCompleted = true;

            this.LogRemoved();
        }

        public TSaga Saga { get; }
    }
}
