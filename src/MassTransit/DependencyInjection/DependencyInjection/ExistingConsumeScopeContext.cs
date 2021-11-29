namespace MassTransit.DependencyInjection
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;


    public class ExistingConsumeScopeContext :
        IConsumeScopeContext
    {
        public ExistingConsumeScopeContext(ConsumeContext context)
        {
            Context = context;
        }

        public ConsumeContext Context { get; }

        public ValueTask DisposeAsync()
        {
            return default;
        }
    }


    public class ExistingConsumeScopeContext<TMessage> :
        IConsumeScopeContext<TMessage>
        where TMessage : class
    {
        readonly IServiceScope _scope;

        public ExistingConsumeScopeContext(ConsumeContext<TMessage> context, IServiceScope scope)
        {
            Context = context;
            _scope = scope;
        }

        public ValueTask DisposeAsync()
        {
            return default;
        }

        public T GetService<T>()
            where T : class
        {
            return ActivatorUtilities.GetServiceOrCreateInstance<T>(_scope.ServiceProvider);
        }

        public ConsumeContext<TMessage> Context { get; }
    }

}
