namespace MassTransit.MartenIntegration
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using Marten;
    using Saga;
    using Util;


    public class MartenSagaConsumeContext<TSaga, TMessage> :
        ConsumeContextProxyScope<TMessage>,
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

            LogContext.Debug?.Log("SAGA:{SagaType}:{CorrelationId} Removed {MessageType}", TypeMetadataCache<TSaga>.ShortName, Saga.CorrelationId,
                TypeMetadataCache<TMessage>.ShortName);
        }

        public TSaga Saga { get; }
        public bool IsCompleted { get; private set; }
    }
}
