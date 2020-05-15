namespace MassTransit.Context
{
    using System;
    using System.Threading.Tasks;
    using Saga;


    public class DefaultSagaConsumeContext<TSaga, TMessage> :
        ConsumeContextScope<TMessage>,
        SagaConsumeContext<TSaga, TMessage>
        where TMessage : class
        where TSaga : class, ISaga
    {
        bool _isCompleted;

        public DefaultSagaConsumeContext(ConsumeContext<TMessage> context, TSaga instance, SagaConsumeContextMode mode)
            : base(context)
        {
            Saga = instance;
        }

        Guid? MessageContext.CorrelationId => Saga.CorrelationId;

        bool SagaConsumeContext<TSaga>.IsCompleted => _isCompleted;

        async Task SagaConsumeContext<TSaga>.SetCompleted()
        {
            _isCompleted = true;
        }

        public TSaga Saga { get; }
    }
}
