namespace MassTransit.Registration
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using Definition;
    using Internals.Extensions;
    using Metadata;
    using Saga;


    public static class SagaDefinitionRegistrationCache
    {
        static CachedRegistration GetOrAdd(Type type)
        {
            if (!type.HasInterface(typeof(ISagaDefinition<>)))
                throw new ArgumentException($"The type is not a saga definition: {TypeMetadataCache.GetShortName(type)}", nameof(type));

            var sagaType = type.GetClosingArguments(typeof(ISagaDefinition<>)).Single();

            return Cached.Instance.GetOrAdd(sagaType,
                _ => (CachedRegistration)Activator.CreateInstance(typeof(CachedRegistration<,>).MakeGenericType(type, sagaType)));
        }

        public static void Register(Type sagaDefinitionType, IContainerRegistrar registrar)
        {
            GetOrAdd(sagaDefinitionType).Register(registrar);
        }


        static class Cached
        {
            internal static readonly ConcurrentDictionary<Type, CachedRegistration> Instance = new ConcurrentDictionary<Type, CachedRegistration>();
        }


        interface CachedRegistration
        {
            void Register(IContainerRegistrar registrar);
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
