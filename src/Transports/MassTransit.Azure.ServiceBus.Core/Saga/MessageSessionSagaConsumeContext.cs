namespace MassTransit.Azure.ServiceBus.Core.Saga
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using MassTransit.Saga;
    using Metadata;


    public class MessageSessionSagaConsumeContext<TSaga, TMessage> :
        ConsumeContextScope<TMessage>,
        SagaConsumeContext<TSaga, TMessage>
        where TMessage : class
        where TSaga : class, ISaga
    {
        readonly MessageSessionContext _sessionContext;

        public MessageSessionSagaConsumeContext(ConsumeContext<TMessage> context, MessageSessionContext sessionContext, TSaga instance)
            : base(context)
        {
            _sessionContext = sessionContext;

            Saga = instance;
        }

        public override Guid? CorrelationId => Saga.CorrelationId;

        public async Task SetCompleted()
        {
            await RemoveState().ConfigureAwait(false);

            IsCompleted = true;

            LogContext.Debug?.Log("SAGA:{SagaType}:{CorrelationId} Removed {MessageType}", TypeMetadataCache<TSaga>.ShortName, Saga.CorrelationId,
                TypeMetadataCache<TMessage>.ShortName);
        }

        public bool IsCompleted { get; private set; }
        public TSaga Saga { get; }

        Task RemoveState()
        {
            return _sessionContext.SetStateAsync(new byte[0]);
        }
    }
}
