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
        readonly IndexedSagaDictionary<TSaga> _sagas;
        readonly SagaInstance<TSaga> _instance;

        public InMemorySagaConsumeContext(ConsumeContext<TMessage> context, SagaInstance<TSaga> instance, IndexedSagaDictionary<TSaga> sagas)
            : base(context)
        {
            _sagas = sagas;
            _instance = instance;
        }

        public override Guid? CorrelationId => Saga.CorrelationId;

        public bool IsCompleted { get; private set; }

        public TSaga Saga => _instance.Instance;

        public async Task SetCompleted()
        {
            _instance.Remove();

            await _sagas.MarkInUse(CancellationToken).ConfigureAwait(false);
            try
            {
                _sagas.Remove(_instance);
            }
            finally
            {
                _sagas.Release();
            }

            IsCompleted = true;

            this.LogRemoved();
        }

        public void Dispose()
        {
            _instance.Release();
        }
    }
}
