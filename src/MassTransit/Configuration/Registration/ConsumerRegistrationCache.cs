namespace MassTransit.Registration
{
    using System;
    using System.Collections.Concurrent;


    public static class ConsumerRegistrationCache
    {
        static CachedRegistration GetOrAdd(Type type)
        {
            return Cached.Instance.GetOrAdd(type, _ =>
                (CachedRegistration)Activator.CreateInstance(typeof(CachedRegistration<>).MakeGenericType(type)));
        }

        public static void Register(Type consumerType, IContainerRegistrar registrar)
        {
            GetOrAdd(consumerType).Register(registrar);
        }


        static class Cached
        {
            internal static readonly ConcurrentDictionary<Type, CachedRegistration> Instance = new ConcurrentDictionary<Type, CachedRegistration>();
        }


        interface CachedRegistration
        {
            void Register(IContainerRegistrar registrar);
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
