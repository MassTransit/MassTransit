namespace MassTransit.Registration
{
    using System;
    using System.Linq;
    using Definition;
    using Internals.Extensions;
    using Metadata;


    public static class ConsumerDefinitionRegistrationCache
    {
        public static void Register(Type consumerDefinitionType, IContainerRegistrar registrar)
        {
            Cached.Instance.GetOrAdd(consumerDefinitionType).Register(registrar);
        }

        static CachedRegistration Factory(Type type)
        {
            if (!type.HasInterface(typeof(IConsumerDefinition<>)))
                throw new ArgumentException($"The type is not a consumer definition: {TypeMetadataCache.GetShortName(type)}", nameof(type));

            var consumerType = type.GetClosingArguments(typeof(IConsumerDefinition<>)).Single();

            return (CachedRegistration)Activator.CreateInstance(typeof(CachedRegistration<,>).MakeGenericType(type, consumerType));
        }


        static class Cached
        {
            internal static readonly ContainerRegistrationCache Instance = new ContainerRegistrationCache(Factory);
        }


        class CachedRegistration<TDefinition, TConsumer> :
            CachedRegistration
            where TDefinition : class, IConsumerDefinition<TConsumer>
            where TConsumer : class, IConsumer
        {
            public void Register(IContainerRegistrar registrar)
            {
                registrar.RegisterConsumerDefinition<TDefinition, TConsumer>();
            }
        }
    }
}
