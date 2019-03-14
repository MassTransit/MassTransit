namespace MassTransit.SimpleInjectorIntegration.Registration
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
    }
}