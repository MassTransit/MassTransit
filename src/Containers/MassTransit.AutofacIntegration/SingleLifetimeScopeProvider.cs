namespace MassTransit.AutofacIntegration
{
    using Autofac;


    public class SingleLifetimeScopeProvider :
        ILifetimeScopeProvider
    {
        readonly ILifetimeScope _scope;

        public SingleLifetimeScopeProvider(ILifetimeScope scope)
        {
            _scope = scope;
        }

        public ILifetimeScope LifetimeScope => _scope;

        public ILifetimeScope GetLifetimeScope(ConsumeContext context)
        {
            return _scope;
        }

        ILifetimeScope ILifetimeScopeProvider.GetLifetimeScope<T>(ConsumeContext<T> context)
        {
            return _scope;
        }
    }
}
