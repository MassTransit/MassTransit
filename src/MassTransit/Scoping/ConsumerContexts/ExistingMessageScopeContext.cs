namespace MassTransit.Scoping.ConsumerContexts
{
    using System.Threading.Tasks;


    public class ExistingMessageScopeContext<T> :
        IMessageScopeContext<T>
        where T : class
    {
        public ExistingMessageScopeContext(ConsumeContext<T> context)
        {
            Context = context;
        }

        public ValueTask DisposeAsync()
        {
            return default;
        }

        public ConsumeContext<T> Context { get; }
    }
}
