namespace MassTransit.Scoping.SendContexts
{
    using System;


    public class CreatedSendScopeContext<TScope, T> :
        ISendScopeContext<T>
        where TScope : IDisposable
        where T : class
    {
        readonly TScope _scope;

        public CreatedSendScopeContext(TScope scope, SendContext<T> context)
        {
            _scope = scope;
            Context = context;
        }

        public void Dispose()
        {
            _scope?.Dispose();
        }

        public SendContext<T> Context { get; }
    }
}
