namespace MassTransit.KafkaIntegration
{
    using System;
    using Confluent.Kafka;
    using ExtensionsDependencyInjectionIntegration;
    using ExtensionsDependencyInjectionIntegration.MultiBus;
    using Microsoft.Extensions.DependencyInjection;
    using Monitoring.Health;
    using Registration;


    public static class KafkaIntegrationExtensions
    {
        static IRegistrationContext<IServiceProvider> CreateRegistrationContext(IServiceProvider provider)
        {
            return new RegistrationContext<IServiceProvider>(provider.GetRequiredService<IRegistration>(), provider.GetService<BusHealth>(), provider);
        }

        public static void AttachKafka<TBus>(this IServiceCollectionConfigurator<TBus> configurator, Action<IRegistration, IKafkaFactoryConfigurator> configure)
            where TBus : class, IBus
        {
            //TODO: move to registration configurator
            var factory = new KafkaBusInstanceConfiguratorFactory<IServiceProvider>(configure);
            configurator.Collection.AddSingleton(provider => Bind<TBus>.Create(factory.CreateBusInstanceConfigurator(CreateRegistrationContext(provider))));
        }

        public static void AttachKafka<TBus>(this IServiceCollectionConfigurator<TBus> configurator, ClientConfig clientConfig,
            Action<IRegistration, IKafkaFactoryConfigurator> configure)
            where TBus : class, IBus
        {
            //TODO: move to registration configurator
            var factory = new KafkaBusInstanceConfiguratorFactory<IServiceProvider>(clientConfig, configure);
            configurator.Collection.AddSingleton(provider => Bind<TBus>.Create(factory.CreateBusInstanceConfigurator(CreateRegistrationContext(provider))));
        }

        public static void AttachKafka(this IServiceCollectionConfigurator configurator, Action<IRegistration, IKafkaFactoryConfigurator> configure)
        {
            //TODO: move to registration configurator
            var factory = new KafkaBusInstanceConfiguratorFactory<IServiceProvider>(configure);
            configurator.Collection.AddSingleton(provider => Bind<IBus>.Create(factory.CreateBusInstanceConfigurator(CreateRegistrationContext(provider))));
        }

        public static void AttachKafka(this IServiceCollectionConfigurator configurator, ClientConfig clientConfig,
            Action<IRegistration, IKafkaFactoryConfigurator> configure)
        {
            //TODO: move to registration configurator
            var factory = new KafkaBusInstanceConfiguratorFactory<IServiceProvider>(clientConfig, configure);
            configurator.Collection.AddSingleton(provider => Bind<IBus>.Create(factory.CreateBusInstanceConfigurator(CreateRegistrationContext(provider))));
        }
    }
}
