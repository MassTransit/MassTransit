namespace MassTransit.AutofacIntegration
{
    using Autofac;


    public interface ILifetimeScopeProvider
    {
        ILifetimeScope LifetimeScope { get; }

        ILifetimeScope GetLifetimeScope(ConsumeContext context);

        ILifetimeScope GetLifetimeScope<T>(ConsumeContext<T> context)
            where T : class;
    }
}
