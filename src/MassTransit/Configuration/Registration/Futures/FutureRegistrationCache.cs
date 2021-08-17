namespace MassTransit.Registration
{
    using System;
    using Futures;
    using Internals.Extensions;
    using Metadata;


    public static class FutureRegistrationCache
    {
        public static void Register(Type futureType, IContainerRegistrar registrar)
        {
            Cached.Instance.GetOrAdd(futureType).Register(registrar);
        }

        public static IFutureRegistrationConfigurator AddFuture(IRegistrationConfigurator configurator, Type futureType, Type futureDefinitionType)
        {
            return Cached.Instance.GetOrAdd(futureType).AddFuture(configurator, futureDefinitionType);
        }

        static CachedRegistration Factory(Type type)
        {
            if (!type.ClosesType(typeof(Future<,,>), out Type[] types))
                throw new ArgumentException($"Type is not a Future: {TypeMetadataCache.GetShortName(type)}", nameof(type));

            return (CachedRegistration)Activator.CreateInstance(typeof(CachedRegistration<,,,>).MakeGenericType(type, types[0], types[1], types[2]));
        }


        static class Cached
        {
            internal static readonly RegistrationCache<CachedRegistration> Instance = new RegistrationCache<CachedRegistration>(Factory);
        }


        interface CachedRegistration
        {
            void Register(IContainerRegistrar registrar);
            IFutureRegistrationConfigurator AddFuture(IRegistrationConfigurator configurator, Type futureDefinitionType);
        }


        class CachedRegistration<TFuture, TRequest, TResponse, TFault> :
            CachedRegistration
            where TFuture : Future<TRequest, TResponse, TFault>
            where TRequest : class
            where TResponse : class
            where TFault : class
        {
            public void Register(IContainerRegistrar registrar)
            {
                registrar.RegisterFuture<TFuture>();
            }

            public IFutureRegistrationConfigurator AddFuture(IRegistrationConfigurator configurator, Type futureDefinitionType)
            {
                return configurator.AddFuture<TFuture>(futureDefinitionType ?? typeof(DefaultFutureDefinition<TFuture>));
            }
        }
    }
}
