namespace MassTransit.Scoping.SagaContexts
{
    using System;


    public class CreatedSagaScopeContext<TScope, T> :
        ISagaScopeContext<T>
        where TScope : IDisposable
        where T : class
    {
        public CreatedSagaScopeContext(TScope scope, ConsumeContext<T> context)
        {
            Scope = scope;
            Context = context;
        }

        public void Dispose()
        {
            Scope.Dispose();
        }

        public TScope Scope { get; }
        public ConsumeContext<T> Context { get; }
    }
}