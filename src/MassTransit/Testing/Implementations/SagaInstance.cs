namespace MassTransit.Testing.Implementations
{
    using System;


    public class SagaInstance<T> :
        ISagaInstance<T>
        where T : class, ISaga
    {
        public SagaInstance(T saga)
        {
            Saga = saga;
        }

        public T Saga { get; }

        public Guid? ElementId => Saga.CorrelationId;
    }
}
