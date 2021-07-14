namespace MassTransit.Scoping.ConsumerContexts
{
    using System;
    using System.Threading.Tasks;


    public class ExistingConsumerScopeContext :
        IConsumerScopeContext
    {
        public ExistingConsumerScopeContext(ConsumeContext context)
        {
            Context = context;
        }

        public ConsumeContext Context { get; }

        public ValueTask DisposeAsync()
        {
            return default;
        }
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

        public ConsumerConsumeContext<TConsumer, T> Context { get; }

        public ValueTask DisposeAsync()
        {
            _disposeCallback?.Invoke(Context.Consumer);

            return default;
        }
    }
}
