namespace MassTransit.Registration
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using Definition;
    using Internals.Extensions;
    using Metadata;


    public static class ConsumerDefinitionRegistrationCache
    {
        static CachedRegistration GetOrAdd(Type type)
        {
            if (!type.HasInterface(typeof(IConsumerDefinition<>)))
                throw new ArgumentException($"The type is not a consumer definition: {TypeMetadataCache.GetShortName(type)}", nameof(type));

            var consumerType = type.GetClosingArguments(typeof(IConsumerDefinition<>)).Single();

            return Cached.Instance.GetOrAdd(consumerType,
                _ => (CachedRegistration)Activator.CreateInstance(typeof(CachedRegistration<,>).MakeGenericType(type, consumerType)));
        }

        public static void Register(Type consumerDefinitionType, IContainerRegistrar registrar)
        {
            GetOrAdd(consumerDefinitionType).Register(registrar);
        }


        static class Cached
        {
            internal static readonly ConcurrentDictionary<Type, CachedRegistration> Instance = new ConcurrentDictionary<Type, CachedRegistration>();
        }


        interface CachedRegistration
        {
            void Register(IContainerRegistrar registrar);
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
