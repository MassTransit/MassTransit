namespace MassTransit.Registration
{
    using System;
    using System.Linq;
    using Definition;
    using Internals.Extensions;
    using Metadata;
    using Saga;


    public static class SagaDefinitionRegistrationCache
    {
        public static void Register(Type sagaDefinitionType, IContainerRegistrar registrar)
        {
            Cached.Instance.GetOrAdd(sagaDefinitionType).Register(registrar);
        }

        static CachedRegistration Factory(Type type)
        {
            if (!type.HasInterface(typeof(ISagaDefinition<>)))
                throw new ArgumentException($"The type is not a saga definition: {TypeMetadataCache.GetShortName(type)}", nameof(type));

            var sagaType = type.GetClosingArguments(typeof(ISagaDefinition<>)).Single();

            return (CachedRegistration)Activator.CreateInstance(typeof(CachedRegistration<,>).MakeGenericType(type, sagaType));
        }


        static class Cached
        {
            internal static readonly ContainerRegistrationCache Instance = new ContainerRegistrationCache(Factory);
        }


        class CachedRegistration<TDefinition, TSaga> :
            CachedRegistration
            where TDefinition : class, ISagaDefinition<TSaga>
            where TSaga : class, ISaga
        {
            public void Register(IContainerRegistrar registrar)
            {
                registrar.RegisterSagaDefinition<TDefinition, TSaga>();
            }
        }
    }
}
