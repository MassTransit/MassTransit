namespace MassTransit.Context
{
    using System;
    using System.Threading.Tasks;
    using Saga;
    using Saga.InMemoryRepository;


    public class InMemorySagaConsumeContext<TSaga, TMessage> :
        ConsumeContextScope<TMessage>,
        SagaConsumeContext<TSaga, TMessage>,
        IDisposable
        where TMessage : class
        where TSaga : class, ISaga
    {
        readonly SagaInstance<TSaga> _instance;

        public InMemorySagaConsumeContext(ConsumeContext<TMessage> context, SagaInstance<TSaga> instance)
            : base(context)
        {
            _instance = instance;
        }

        public void Dispose()
        {
            _instance.Release();
        }

        public override Guid? CorrelationId => Saga.CorrelationId;

        public bool IsCompleted { get; private set; }

        public TSaga Saga => _instance.Instance;

        public async Task SetCompleted()
        {
            IsCompleted = true;
        }
    }
}
