namespace MassTransit.AutofacIntegration.ScopeProviders
{
    using System;
    using Autofac;
    using MassTransit.Registration;


    public class AutofacScopeServiceProvider :
        IScopeServiceProvider
    {
        readonly ILifetimeScope _lifetimeScope;

        public AutofacScopeServiceProvider(ILifetimeScope lifetimeScope)
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
