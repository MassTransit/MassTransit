namespace MassTransit.AutofacIntegration.Registration
{
    using System;
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

        public object GetService(Type serviceType)
        {
            return _lifetimeScope.ResolveOptional(serviceType);
        }
    }
}
