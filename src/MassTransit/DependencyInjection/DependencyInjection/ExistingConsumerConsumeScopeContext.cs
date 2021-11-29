namespace MassTransit.DependencyInjection
{
    using System.Threading.Tasks;


    public class ExistingConsumerConsumeScopeContext<TConsumer, T> :
        IConsumerConsumeScopeContext<TConsumer, T>
        where TConsumer : class
        where T : class
    {
        public ExistingConsumerConsumeScopeContext(ConsumerConsumeContext<TConsumer, T> context)
        {
            Context = context;
        }

        public ConsumerConsumeContext<TConsumer, T> Context { get; }

        public ValueTask DisposeAsync()
        {
            return default;
        }
    }
}
