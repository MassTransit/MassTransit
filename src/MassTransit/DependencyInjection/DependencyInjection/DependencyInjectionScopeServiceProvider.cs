namespace MassTransit.DependencyInjection
{
    using System;


    public class DependencyInjectionScopeServiceProvider :
        IScopeServiceProvider
    {
        readonly IServiceProvider _provider;

        public DependencyInjectionScopeServiceProvider(IServiceProvider provider)
        {
            _provider = provider;
        }

        public object GetService(Type serviceType)
        {
            return _provider.GetService(serviceType);
        }
    }
}
