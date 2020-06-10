namespace MassTransit.Registration
{
    using System;
    using System.Linq;
    using Courier;
    using Definition;
    using Internals.Extensions;
    using Metadata;


    public static class ActivityDefinitionRegistrationCache
    {
        public static void Register(Type sagaDefinitionType, IContainerRegistrar registrar)
        {
            Cached.Instance.GetOrAdd(sagaDefinitionType).Register(registrar);
        }

        static CachedRegistration Factory(Type type)
        {
            if (!type.HasInterface(typeof(IActivityDefinition<,,>)))
                throw new ArgumentException($"The type is not an activity definition: {TypeMetadataCache.GetShortName(type)}", nameof(type));

            Type[] argumentLogTypes = type.GetClosingArguments(typeof(IActivityDefinition<,,>)).ToArray();
            var genericType = typeof(CachedRegistration<,,,>).MakeGenericType(type, argumentLogTypes[0], argumentLogTypes[1], argumentLogTypes[2]);

            return (CachedRegistration)Activator.CreateInstance(genericType);
        }


        static class Cached
        {
            internal static readonly ContainerRegistrationCache Instance = new ContainerRegistrationCache(Factory);
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
