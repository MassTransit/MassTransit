namespace MassTransit.RedisIntegration
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using Saga;


    public class RedisSagaConsumeContext<TSaga, TMessage> :
        ConsumeContextScope<TMessage>,
        SagaConsumeContext<TSaga, TMessage>
        where TMessage : class
        where TSaga : class, IVersionedSaga
    {
        readonly ITypedDatabase<TSaga> _sagas;

        public RedisSagaConsumeContext(ITypedDatabase<TSaga> sagas, ConsumeContext<TMessage> context, TSaga instance)
            : base(context)
        {
            _sagas = sagas;

            Saga = instance;
        }

        Guid? MessageContext.CorrelationId => Saga.CorrelationId;

        async Task SagaConsumeContext<TSaga>.SetCompleted()
        {
            await _sagas.Delete(Saga.CorrelationId).ConfigureAwait(false);

            IsCompleted = true;

            this.LogRemoved();
        }

        public TSaga Saga { get; }
        public bool IsCompleted { get; private set; }
    }
}
