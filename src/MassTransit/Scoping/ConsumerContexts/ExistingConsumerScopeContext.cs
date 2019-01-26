namespace MassTransit.Scoping.ConsumerContexts
{
    using System;


    public class ExistingConsumerScopeContext :
        IConsumerScopeContext
    {
        public ExistingConsumerScopeContext(ConsumeContext context)
        {
            Context = context;
        }

        public void Dispose()
        {
        }

        public ConsumeContext Context { get; }
    }


    public class ExistingConsumerScopeContext<TConsumer, T> :
        IConsumerScopeContext<TConsumer, T>
        where TConsumer : class
        where T : class
    {
        readonly Action<TConsumer> _disposeCallback;

        public ExistingConsumerScopeContext(ConsumerConsumeContext<TConsumer, T> context, Action<TConsumer> disposeCallback = null)
        {
            _disposeCallback = disposeCallback;
            Context = context;
        }

        public void Dispose()
        {
            _disposeCallback?.Invoke(Context.Consumer);
        }

        public ConsumerConsumeContext<TConsumer, T> Context { get; }
    }
}
