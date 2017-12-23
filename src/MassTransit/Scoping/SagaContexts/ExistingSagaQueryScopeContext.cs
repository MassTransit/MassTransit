namespace MassTransit.Scoping.SagaContexts
{
    using Saga;


    public class ExistingSagaQueryScopeContext<TSaga, T> :
        ISagaQueryScopeContext<TSaga, T>
        where TSaga : class, ISaga
        where T : class
    {
        public ExistingSagaQueryScopeContext(SagaQueryConsumeContext<TSaga, T> context)
        {
            Context = context;
        }

        public void Dispose()
        {
        }

        public SagaQueryConsumeContext<TSaga, T> Context { get; }
    }
}