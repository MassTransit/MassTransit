namespace MassTransit.SimpleInjectorIntegration
{
    using System;
    using Scoping;
    using SimpleInjector;


    static class ContainerResolutionExtensions
    {
        public static T TryGetInstance<T>(this Container container)
        {
            IServiceProvider serviceProvider = container;
            var service = serviceProvider.GetService(typeof(T));
            return (T)service;
        }

        public static T TryGetInstance<T>(this Scope scope)
        {
            IServiceProvider serviceProvider = scope;
            var service = serviceProvider.GetService(typeof(T));
            return (T)service;
        }

        public static ConsumeContext GetConsumeContext(this Container container)
        {
            var scope = Lifestyle.Scoped.GetCurrentScope(container);

            return scope?.Container.GetInstance<ScopedConsumeContextProvider>().GetContext();
        }
    }
}
