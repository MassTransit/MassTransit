namespace MassTransit.Saga
{
    using System;
    using Context;


    public class InMemorySagaConsumeContext<TSaga, TMessage> :
        DefaultSagaConsumeContext<TSaga, TMessage>,
        IDisposable
        where TMessage : class
        where TSaga : class, ISaga
    {
        readonly SagaInstance<TSaga> _saga;

        public InMemorySagaConsumeContext(ConsumeContext<TMessage> context, SagaInstance<TSaga> saga)
            : base(context, saga.Instance)
        {
            _saga = saga;
        }

        public void Dispose()
        {
            _saga.Release();
        }
    }
}
