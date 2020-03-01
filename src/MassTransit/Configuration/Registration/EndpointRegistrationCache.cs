namespace MassTransit.Registration
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using Internals.Extensions;
    using Metadata;


    public static class EndpointRegistrationCache
    {
        static CachedRegistration GetOrAdd(Type type)
        {
            if (!type.HasInterface(typeof(IEndpointDefinition<>)))
                throw new ArgumentException($"The type is not an execute activity: {TypeMetadataCache.GetShortName(type)}", nameof(type));

            var targetType = type.GetClosingArguments(typeof(IEndpointDefinition<>)).Single();

            return Cached.Instance.GetOrAdd(type,
                _ => (CachedRegistration)Activator.CreateInstance(typeof(CachedRegistration<,>).MakeGenericType(type, targetType)));
        }

        public static void Register(Type definitionType, IContainerRegistrar registrar)
        {
            GetOrAdd(definitionType).Register(registrar);
        }

        public static IEndpointRegistration CreateRegistration(Type definitionType, IContainerRegistrar registrar)
        {
            return GetOrAdd(definitionType).CreateRegistration(registrar);
        }


        static class Cached
        {
            internal static readonly ConcurrentDictionary<Type, CachedRegistration> Instance = new ConcurrentDictionary<Type, CachedRegistration>();
        }


        interface CachedRegistration
        {
            void Register(IContainerRegistrar registrar);
            IEndpointRegistration CreateRegistration(IContainerRegistrar registrar);
        }


        class CachedRegistration<TDefinition, T> :
            CachedRegistration
            where TDefinition : class, IEndpointDefinition<T>
            where T : class
        {
            public void Register(IContainerRegistrar registrar)
            {
                registrar.RegisterEndpointDefinition<TDefinition, T>();
            }

            public IEndpointRegistration CreateRegistration(IContainerRegistrar registrar)
            {
                Register(registrar);

                return new EndpointRegistration<T>();
            }
        }
    }
}
