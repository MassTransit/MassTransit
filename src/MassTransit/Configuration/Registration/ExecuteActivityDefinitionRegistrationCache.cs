namespace MassTransit.Registration
{
    using System;
    using System.Linq;
    using Courier;
    using Definition;
    using Internals.Extensions;
    using Metadata;


    public static class ExecuteActivityDefinitionRegistrationCache
    {
        public static void Register(Type executeActivityDefinitionType, IContainerRegistrar registrar)
        {
            Cached.Instance.GetOrAdd(executeActivityDefinitionType).Register(registrar);
        }

        static CachedRegistration Factory(Type type)
        {
            if (!type.HasInterface(typeof(IExecuteActivityDefinition<,>)))
                throw new ArgumentException($"The type is not an activity definition: {TypeMetadataCache.GetShortName(type)}", nameof(type));

            Type[] argumentTypes = type.GetClosingArguments(typeof(IExecuteActivityDefinition<,>)).ToArray();
            var genericType = typeof(CachedRegistration<,,>).MakeGenericType(type, argumentTypes[0], argumentTypes[1]);

            return (CachedRegistration)Activator.CreateInstance(genericType);
        }


        static class Cached
        {
            internal static readonly ContainerRegistrationCache Instance = new ContainerRegistrationCache(Factory);
        }


        class CachedRegistration<TDefinition, TActivity, TArguments> :
            CachedRegistration
            where TDefinition : class, IExecuteActivityDefinition<TActivity, TArguments>
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            public void Register(IContainerRegistrar registrar)
            {
                registrar.RegisterExecuteActivityDefinition<TDefinition, TActivity, TArguments>();
            }
        }
    }
}
