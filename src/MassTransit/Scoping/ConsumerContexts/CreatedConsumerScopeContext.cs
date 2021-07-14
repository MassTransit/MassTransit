namespace MassTransit.Scoping.ConsumerContexts
{
    using System;
    using System.Threading.Tasks;


    public class CreatedConsumerScopeContext<TScope> :
        IConsumerScopeContext
        where TScope : IDisposable
    {
        readonly TScope _scope;

        public CreatedConsumerScopeContext(TScope scope, ConsumeContext context)
        {
            _scope = scope;
            Context = context;
        }

        public ConsumeContext Context { get; }

        public async ValueTask DisposeAsync()
        {
            if (_scope is IAsyncDisposable asyncDisposable)
                await asyncDisposable.DisposeAsync().ConfigureAwait(false);
            else
                _scope?.Dispose();
        }
    }


    public class CreatedConsumerScopeContext<TScope, TConsumer, T> :
        IConsumerScopeContext<TConsumer, T>
        where TScope : IDisposable
        where TConsumer : class
        where T : class
    {
        readonly Action<TConsumer> _disposeCallback;
        readonly TScope _scope;

        public CreatedConsumerScopeContext(TScope scope, ConsumerConsumeContext<TConsumer, T> context, Action<TConsumer> disposeCallback = null)
        {
            _scope = scope;
            _disposeCallback = disposeCallback;
            Context = context;
        }

        public ConsumerConsumeContext<TConsumer, T> Context { get; }

        public async ValueTask DisposeAsync()
        {
            _disposeCallback?.Invoke(Context.Consumer);

            if (_scope is IAsyncDisposable asyncDisposable)
                await asyncDisposable.DisposeAsync().ConfigureAwait(false);
            else
                _scope?.Dispose();
        }
    }
}
