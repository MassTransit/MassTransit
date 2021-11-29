namespace MassTransit.DependencyInjection.Registration
{
    using System;
    using Configuration;


    public static class RequestClientRegistrationCache
    {
        public static void Register(Type requestType, RequestTimeout timeout, IContainerRegistrar registrar)
        {
            Cached.Instance.GetOrAdd(requestType).Register(timeout, registrar);
        }

        public static void Register(Type requestType, Uri destinationAddress, RequestTimeout timeout, IContainerRegistrar registrar)
        {
            Cached.Instance.GetOrAdd(requestType).Register(destinationAddress, timeout, registrar);
        }

        static CachedRegistration Factory(Type type)
        {
            return (CachedRegistration)Activator.CreateInstance(typeof(CachedRegistration<>).MakeGenericType(type));
        }


        static class Cached
        {
            internal static readonly RegistrationCache<CachedRegistration> Instance = new RegistrationCache<CachedRegistration>(Factory);
        }


        interface CachedRegistration
        {
            void Register(RequestTimeout timeout, IContainerRegistrar registrar);
            void Register(Uri destinationAddress, RequestTimeout timeout, IContainerRegistrar registrar);
        }


        class CachedRegistration<T> :
            CachedRegistration
            where T : class
        {
            public void Register(RequestTimeout timeout, IContainerRegistrar registrar)
            {
                registrar.RegisterRequestClient<T>(timeout);
            }

            public void Register(Uri destinationAddress, RequestTimeout timeout, IContainerRegistrar registrar)
            {
                registrar.RegisterRequestClient<T>(destinationAddress, timeout);
            }
        }
    }
}
