namespace MassTransit.Registration
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using Courier;
    using Definition;
    using Internals.Extensions;
    using Metadata;


    public static class ActivityDefinitionRegistrationCache
    {
        static CachedRegistration GetOrAdd(Type type)
        {
            if (!type.HasInterface(typeof(IActivityDefinition<,,>)))
                throw new ArgumentException($"The type is not an activity definition: {TypeMetadataCache.GetShortName(type)}", nameof(type));

            var argumentLogTypes = type.GetClosingArguments(typeof(IActivityDefinition<,,>)).ToArray();
            var genericType = typeof(CachedRegistration<,,,>).MakeGenericType(type, argumentLogTypes[0], argumentLogTypes[1], argumentLogTypes[2]);

            return Cached.Instance.GetOrAdd(type, _ => (CachedRegistration)Activator.CreateInstance(genericType));
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


        class CachedRegistration<TDefinition, TActivity, TArguments, TLog> :
            CachedRegistration
            where TDefinition : class, IActivityDefinition<TActivity, TArguments, TLog>
            where TActivity : class, IActivity<TArguments, TLog>
            where TArguments : class
            where TLog : class
        {
            public void Register(IContainerRegistrar registrar)
            {
                registrar.RegisterActivityDefinition<TDefinition, TActivity, TArguments, TLog>();
            }
        }
    }
}
