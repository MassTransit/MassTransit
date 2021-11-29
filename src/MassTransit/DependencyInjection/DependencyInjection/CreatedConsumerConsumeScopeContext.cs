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
        readonly IServiceScope _scope;

        public CreatedConsumerConsumeScopeContext(IServiceScope scope, ConsumerConsumeContext<TConsumer, T> context)
        {
            _scope = scope;
            Context = context;
        }

        public ConsumerConsumeContext<TConsumer, T> Context { get; }

        public ValueTask DisposeAsync()
        {
            if (_scope is IAsyncDisposable asyncDisposable)
                return asyncDisposable.DisposeAsync();

            _scope?.Dispose();
            return default;
        }
    }
}
