namespace MassTransit.SimpleInjectorIntegration
{
    using System;
    using SimpleInjector;


    static class ContainerResolutionExtensions
    {
        public static T TryGetInstance<T>(this Container container)
        {
            IServiceProvider serviceProvider = container;
            var service = serviceProvider.GetService(typeof(T));
            return (T)service;
        }

        public static ConsumeContext GetConsumeContext(this Container container)
        {
            var scope = Lifestyle.Scoped.GetCurrentScope(container) != null;
            if (scope)
            {
                var consumeContext = container.TryGetInstance<ConsumeContext>();
                if (consumeContext != null)
                    return consumeContext;
            }

            return null;
        }
    }
}
