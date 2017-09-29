namespace MassTransit.Scoping.SagaContexts
{
    using System;
    using Saga;


    public class CreatedSagaQueryScopeContext<TScope, TSaga, T> :
        ISagaQueryScopeContext<TSaga, T>
        where TScope : IDisposable
        where TSaga : class, ISaga
        where T : class
    {
        public CreatedSagaQueryScopeContext(TScope scope, SagaQueryConsumeContext<TSaga, T> context)
        {
            Scope = scope;
            Context = context;
        }

        public void Dispose()
        {
            Scope.Dispose();
        }

        public TScope Scope { get; }
        public SagaQueryConsumeContext<TSaga, T> Context { get; }
    }
}