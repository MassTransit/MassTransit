namespace MassTransit.DependencyInjection
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;


    public class ExistingConsumeScopeContext :
        IConsumeScopeContext
    {
        readonly IDisposable _disposable;

        public ExistingConsumeScopeContext(ConsumeContext context, IDisposable disposable)
        {
            _disposable = disposable;
            Context = context;
        }

        public ConsumeContext Context { get; }

        public ValueTask DisposeAsync()
        {
            _disposable?.Dispose();
            return default;
        }
    }


    public class ExistingConsumeScopeContext<TMessage> :
        IConsumeScopeContext<TMessage>
        where TMessage : class
    {
        readonly IDisposable _disposable;
        readonly IServiceScope _scope;
        readonly ISetScopedConsumeContext _setter;

        public ExistingConsumeScopeContext(ConsumeContext<TMessage> context, IServiceScope scope, IDisposable disposable, ISetScopedConsumeContext setter)
        {
            Context = context;
            _scope = scope;
            _disposable = disposable;
            _setter = setter;
        }

        public ValueTask DisposeAsync()
        {
            _disposable?.Dispose();
            return default;
        }

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

        public ConsumeContext<TMessage> Context { get; }
    }
}
