namespace MassTransit.Scoping.ConsumerContexts
{
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


    public class ExistingConsumerScopeContext<TConsumer,T> :
        IConsumerScopeContext<TConsumer,T>
        where TConsumer : class
        where T : class
    {
        public ExistingConsumerScopeContext(ConsumerConsumeContext<TConsumer, T> context)
        {
            Context = context;
        }

        public void Dispose()
        {
        }

        public ConsumerConsumeContext<TConsumer, T> Context { get; }
    }
}