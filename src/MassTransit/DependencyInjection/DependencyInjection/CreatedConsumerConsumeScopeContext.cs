namespace MassTransit.DependencyInjection
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;


    public class CreatedConsumerConsumeScopeContext<TConsumer, T> :
        IConsumerConsumeScopeContext<TConsumer, T>
        where TConsumer : class
        where T : class
    {
        readonly IDisposable _disposable;
        readonly IServiceScope _scope;

        public CreatedConsumerConsumeScopeContext(IServiceScope scope, ConsumerConsumeContext<TConsumer, T> context, IDisposable disposable)
        {
            _scope = scope;
            _disposable = disposable;
            Context = context;
        }

        public ConsumerConsumeContext<TConsumer, T> Context { get; }

        public ValueTask DisposeAsync()
        {
            _disposable?.Dispose();

            if (_scope is IAsyncDisposable asyncDisposable)
                return asyncDisposable.DisposeAsync();

            _scope?.Dispose();
            return default;
        }
    }
}
