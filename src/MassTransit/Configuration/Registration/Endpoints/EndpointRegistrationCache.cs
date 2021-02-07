namespace MassTransit.Registration
{
    using System;
    using System.Linq;
    using Internals.Extensions;
    using Metadata;


    public static class EndpointRegistrationCache
    {
        public static IEndpointRegistration CreateRegistration(Type definitionType, IContainerRegistrar registrar)
        {
            return Cached.Instance.GetOrAdd(definitionType).CreateRegistration(registrar);
        }

        static CachedRegistration Factory(Type type)
        {
            if (!type.HasInterface(typeof(IEndpointDefinition<>)))
                throw new ArgumentException($"The type is not an execute activity: {TypeMetadataCache.GetShortName(type)}", nameof(type));

            var targetType = type.GetClosingArguments(typeof(IEndpointDefinition<>)).Single();

            return (CachedRegistration)Activator.CreateInstance(typeof(CachedRegistration<,>).MakeGenericType(type, targetType));
        }


        static class Cached
        {
            internal static readonly RegistrationCache<CachedRegistration> Instance = new RegistrationCache<CachedRegistration>(Factory);
        }


        interface CachedRegistration
        {
            IEndpointRegistration CreateRegistration(IContainerRegistrar registrar);
        }


        class CachedRegistration<TDefinition, T> :
            CachedRegistration
            where TDefinition : class, IEndpointDefinition<T>
            where T : class
        {
            public IEndpointRegistration CreateRegistration(IContainerRegistrar registrar)
            {
                registrar.RegisterEndpointDefinition<TDefinition, T>();

                return new EndpointRegistration<T>();
            }
        }
    }
}
