namespace MassTransit.Registration
{
    using System;


    public static class ConsumerRegistrationCache
    {
        public static void Register(Type consumerType, IContainerRegistrar registrar)
        {
            Cached.Instance.GetOrAdd(consumerType).Register(registrar);
        }

        static CachedRegistration Factory(Type type)
        {
            return (CachedRegistration)Activator.CreateInstance(typeof(CachedRegistration<>).MakeGenericType(type));
        }


        static class Cached
        {
            internal static readonly ContainerRegistrationCache Instance = new ContainerRegistrationCache(Factory);
        }


        class CachedRegistration<T> :
            CachedRegistration
            where T : class, IConsumer
        {
            public void Register(IContainerRegistrar registrar)
            {
                registrar.RegisterConsumer<T>();
            }
        }
    }
}
