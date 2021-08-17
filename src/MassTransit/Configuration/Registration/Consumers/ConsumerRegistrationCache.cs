namespace MassTransit.Registration
{
    using System;


    public static class ConsumerRegistrationCache
    {
        public static void Register(Type consumerType, IContainerRegistrar registrar)
        {
            Cached.Instance.GetOrAdd(consumerType).Register(registrar);
        }

        public static IConsumerRegistrationConfigurator AddConsumer(IRegistrationConfigurator configurator, Type consumerType,
            Type consumerDefinitionType = null)
        {
            return Cached.Instance.GetOrAdd(consumerType).AddConsumer(configurator, consumerDefinitionType);
        }

        static CachedRegistration Factory(Type type)
        {
            return (CachedRegistration)Activator.CreateInstance(typeof(CachedRegistration<>).MakeGenericType(type));
        }


        static class Cached
        {
            internal static readonly RegistrationCache<CachedRegistration> Instance = new RegistrationCache<CachedRegistration>(Factory);
        }


        interface CachedRegistration
        {
            IConsumerRegistrationConfigurator AddConsumer(IRegistrationConfigurator configurator, Type consumerDefinitionType);
            void Register(IContainerRegistrar registrar);
        }


        class CachedRegistration<T> :
            CachedRegistration
            where T : class, IConsumer
        {
            public void Register(IContainerRegistrar registrar)
            {
                registrar.RegisterConsumer<T>();
            }

            public IConsumerRegistrationConfigurator AddConsumer(IRegistrationConfigurator configurator, Type consumerDefinitionType)
            {
                IConsumerRegistrationConfigurator<T> registrationConfigurator = configurator.AddConsumer<T>(consumerDefinitionType);

                return registrationConfigurator;
            }
        }
    }
}
