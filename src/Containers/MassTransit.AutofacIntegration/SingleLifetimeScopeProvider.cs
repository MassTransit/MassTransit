namespace MassTransit.AutofacIntegration
{
    using Autofac;


    public class SingleLifetimeScopeProvider :
        ILifetimeScopeProvider
    {
        public SingleLifetimeScopeProvider(ILifetimeScope lifetimeScope)
        {
            LifetimeScope = lifetimeScope;
        }

        public ILifetimeScope LifetimeScope { get; }

        public ILifetimeScope GetLifetimeScope<T>(SendContext<T> context)
            where T : class
        {
            return LifetimeScope;
        }

        public ILifetimeScope GetLifetimeScope<T>(PublishContext<T> context)
            where T : class
        {
            return LifetimeScope;
        }

        public ILifetimeScope GetLifetimeScope(ConsumeContext context)
        {
            return LifetimeScope;
        }

        ILifetimeScope ILifetimeScopeProvider.GetLifetimeScope<T>(ConsumeContext<T> context)
        {
            return LifetimeScope;
        }
    }
}
