namespace MassTransit.Scoping.SendContexts
{
    public class ExistingSendScopeContext<T> :
        ISendScopeContext<T>
        where T : class
    {
        public ExistingSendScopeContext(SendContext<T> context)
        {
            Context = context;
        }

        public void Dispose()
        {
        }

        public SendContext<T> Context { get; }
    }
}
