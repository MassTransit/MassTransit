namespace MassTransit.Scoping.ConsumerContexts
{
    using System;


    public class CreatedMessageScopeContext<TScope, T> :
        IMessageScopeContext<T>
        where TScope : IDisposable
        where T : class
    {
        readonly TScope _scope;

        public CreatedMessageScopeContext(TScope scope, ConsumeContext<T> context)
        {
            _scope = scope;
            Context = context;
        }

        public void Dispose()
        {
            _scope?.Dispose();
        }

        public ConsumeContext<T> Context { get; }
    }
}
