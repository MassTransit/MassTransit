namespace MassTransit.Scoping.ConsumerContexts
{
    using System;


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

        public void Dispose()
        {
            _scope?.Dispose();
        }

        public ConsumeContext Context { get; }
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

        public void Dispose()
        {
            _disposeCallback?.Invoke(Context.Consumer);
            _scope?.Dispose();
        }

        public ConsumerConsumeContext<TConsumer, T> Context { get; }
    }
}
