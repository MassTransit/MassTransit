namespace MassTransit.DependencyInjection
{
    using System;
    using System.Threading.Tasks;


    public class ExistingConsumerConsumeScopeContext<TConsumer, T> :
        IConsumerConsumeScopeContext<TConsumer, T>
        where TConsumer : class
        where T : class
    {
        readonly IDisposable _disposable;

        public ExistingConsumerConsumeScopeContext(ConsumerConsumeContext<TConsumer, T> context, IDisposable disposable)
        {
            _disposable = disposable;
            Context = context;
        }

        public ConsumerConsumeContext<TConsumer, T> Context { get; }

        public async ValueTask DisposeAsync()
        {
            _disposable?.Dispose();
        }
    }
}
