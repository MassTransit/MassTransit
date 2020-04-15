namespace MassTransit.AutofacIntegration
{
    using Autofac;


    public interface ILifetimeScopeProvider
    {
        ILifetimeScope LifetimeScope { get; }

        ILifetimeScope GetLifetimeScope<T>(SendContext<T> context)
            where T : class;

        ILifetimeScope GetLifetimeScope<T>(PublishContext<T> context)
            where T : class;

        ILifetimeScope GetLifetimeScope(ConsumeContext context);

        ILifetimeScope GetLifetimeScope<T>(ConsumeContext<T> context)
            where T : class;
    }
}
