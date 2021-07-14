namespace MassTransit.Scoping.ConsumerContexts
{
    using System;
    using System.Threading.Tasks;


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

        public ConsumeContext<T> Context { get; }

        public async ValueTask DisposeAsync()
        {
            if (_scope is IAsyncDisposable asyncDisposable)
                await asyncDisposable.DisposeAsync().ConfigureAwait(false);
            else
                _scope?.Dispose();
        }
    }
}
