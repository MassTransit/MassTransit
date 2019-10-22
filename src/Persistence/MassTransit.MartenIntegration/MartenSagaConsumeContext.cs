namespace MassTransit.MartenIntegration
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using Marten;
    using Saga;


    public class MartenSagaConsumeContext<TSaga, TMessage> :
        ConsumeContextScope<TMessage>,
        SagaConsumeContext<TSaga, TMessage>
        where TMessage : class
        where TSaga : class, ISaga
    {
        readonly IDocumentSession _session;

        public MartenSagaConsumeContext(IDocumentSession session,
            ConsumeContext<TMessage> context, TSaga instance)
            : base(context)
        {
            _session = session;
            Saga = instance;
        }

        Guid? MessageContext.CorrelationId => Saga.CorrelationId;

        async Task SagaConsumeContext<TSaga>.SetCompleted()
        {
            _session.Delete(Saga);
            await _session.SaveChangesAsync().ConfigureAwait(false);

            IsCompleted = true;

            this.LogRemoved();
        }

        public TSaga Saga { get; }
        public bool IsCompleted { get; private set; }
    }
}
