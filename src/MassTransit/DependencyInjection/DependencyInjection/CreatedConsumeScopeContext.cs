namespace MassTransit.DependencyInjection
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;


    public class CreatedConsumeScopeContext :
        IConsumeScopeContext
    {
        readonly IDisposable _disposable;
        readonly IServiceScope _scope;

        public CreatedConsumeScopeContext(IServiceScope scope, ConsumeContext context, IDisposable disposable)
        {
            _scope = scope;
            _disposable = disposable;
            Context = context;
        }

        public ConsumeContext Context { get; }

        public ValueTask DisposeAsync()
        {
            _disposable?.Dispose();

            if (_scope is IAsyncDisposable asyncDisposable)
                return asyncDisposable.DisposeAsync();

            _scope?.Dispose();
            return default;
        }
    }


    public class CreatedConsumeScopeContext<TMessage> :
        IConsumeScopeContext<TMessage>
        where TMessage : class
    {
        readonly IDisposable _disposable;
        readonly IServiceScope _scope;
        readonly ISetScopedConsumeContext _setter;

        public CreatedConsumeScopeContext(IServiceScope scope, ConsumeContext<TMessage> context, IDisposable disposable, ISetScopedConsumeContext setter)
        {
            _scope = scope;
            _disposable = disposable;
            _setter = setter;
            Context = context;
        }

        public ConsumeContext<TMessage> Context { get; }

        public T GetService<T>()
            where T : class
        {
            return ActivatorUtilities.GetServiceOrCreateInstance<T>(_scope.ServiceProvider);
        }

        public T CreateInstance<T>(params object[] arguments)
            where T : class
        {
            return ActivatorUtilities.CreateInstance<T>(_scope.ServiceProvider, arguments);
        }

        public IDisposable PushConsumeContext(ConsumeContext context)
        {
            return _setter.PushContext(_scope, context);
        }

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
