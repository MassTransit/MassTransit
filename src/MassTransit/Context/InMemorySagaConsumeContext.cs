namespace MassTransit.Context
{
    using System;
    using System.Threading.Tasks;
    using Saga;


    public class InMemorySagaConsumeContext<TSaga, TMessage> :
        ConsumeContextScope<TMessage>,
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

            this.LogRemoved();
        }

        public bool IsCompleted { get; private set; }
        public TSaga Saga { get; }
    }
}
