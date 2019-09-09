namespace MassTransit.AutofacIntegration.Registration
{
    using Autofac;
    using MassTransit.Registration;


    public class AutofacConfigurationServiceProvider :
        IConfigurationServiceProvider
    {
        readonly ILifetimeScope _lifetimeScope;

        public AutofacConfigurationServiceProvider(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public T GetRequiredService<T>()
            where T : class
        {
            return _lifetimeScope.Resolve<T>();
        }

        public T GetService<T>()
            where T : class
        {
            return _lifetimeScope.ResolveOptional<T>();
        }
    }
}
