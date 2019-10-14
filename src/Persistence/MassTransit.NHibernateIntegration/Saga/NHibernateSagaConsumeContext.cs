namespace MassTransit.NHibernateIntegration.Saga
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using MassTransit.Saga;
    using NHibernate;


    public class NHibernateSagaConsumeContext<TSaga, TMessage> :
        ConsumeContextScope<TMessage>,
        SagaConsumeContext<TSaga, TMessage>
        where TMessage : class
        where TSaga : class, ISaga
    {
        readonly ISession _session;

        public NHibernateSagaConsumeContext(ISession session, ConsumeContext<TMessage> context, TSaga instance)
            : base(context)
        {
            Saga = instance;
            _session = session;
        }

        Guid? MessageContext.CorrelationId => Saga.CorrelationId;

        async Task SagaConsumeContext<TSaga>.SetCompleted()
        {
            await _session.DeleteAsync(Saga).ConfigureAwait(false);
            IsCompleted = true;

            this.LogRemoved();
        }

        public bool IsCompleted { get; private set; }
        public TSaga Saga { get; }
    }
}
