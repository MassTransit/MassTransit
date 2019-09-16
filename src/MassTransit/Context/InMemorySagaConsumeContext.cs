namespace MassTransit.Context
{
    using System;
    using System.Threading.Tasks;
    using Metadata;
    using Saga;
    using Util;


    public class InMemorySagaConsumeContext<TSaga, TMessage> :
        ConsumeContextProxyScope<TMessage>,
        SagaConsumeContext<TSaga, TMessage>
        where TMessage : class
        where TSaga : class, ISaga
    {
        readonly Func<Task> _removeSaga;

        public InMemorySagaConsumeContext(ConsumeContext<TMessage> context, TSaga instance, Func<Task> removeSaga)
            : base(context)
        {
            _removeSaga = removeSaga;

            Saga = instance;
        }

        public override Guid? CorrelationId => Saga.CorrelationId;

        public async Task SetCompleted()
        {
            await _removeSaga().ConfigureAwait(false);

            IsCompleted = true;

            LogContext.Debug?.Log("SAGA:{SagaType}:{CorrelationId} Removed {MessageType}", TypeMetadataCache<TSaga>.ShortName, Saga.CorrelationId,
                TypeMetadataCache<TMessage>.ShortName);
        }

        public bool IsCompleted { get; private set; }
        public TSaga Saga { get; }
    }
}
