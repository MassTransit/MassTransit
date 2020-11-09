namespace MassTransit.Scoping.ConsumerContexts
{
    public class ExistingMessageScopeContext<T> :
        IMessageScopeContext<T>
        where T : class
    {
        public ExistingMessageScopeContext(ConsumeContext<T> context)
        {
            Context = context;
        }

        public void Dispose()
        {
        }

        public ConsumeContext<T> Context { get; }
    }
}
