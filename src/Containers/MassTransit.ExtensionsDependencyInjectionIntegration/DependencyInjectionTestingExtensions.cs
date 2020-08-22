namespace MassTransit.Testing
{
    using System;
    using Automatonymous;
    using ExtensionsDependencyInjectionIntegration;
    using Microsoft.Extensions.DependencyInjection;
    using Registration;
    using Saga;


    public static class DependencyInjectionTestingExtensions
    {
        /// <summary>
        /// Add the In-Memory test harness to the container, and configure it using the callback specified.
        /// </summary>
        public static IServiceCollection AddMassTransitInMemoryTestHarness(this IServiceCollection services,
            Action<IServiceCollectionBusConfigurator> configure = null)
        {
            services.AddMassTransit(cfg =>
            {
                configure?.Invoke(cfg);

                cfg.SetBusFactory(new InMemoryTestHarnessRegistrationBusFactory());
            });
            services.AddSingleton(provider =>
            {
                var busInstance = provider.GetRequiredService<IBusInstance>();

                if (busInstance is InMemoryTestHarnessBusInstance instance)
                    return instance.Harness;

                throw new ConfigurationException("Test Harness configuration is invalid");
            });
            services.AddSingleton<BusTestHarness>(provider => provider.GetRequiredService<InMemoryTestHarness>());

            return services;
        }

        /// <summary>
        /// Add a consumer test harness for the specified consumer to the container
        /// </summary>
        public static void AddConsumerTestHarness<T>(this IServiceCollectionBusConfigurator configurator)
            where T : class, IConsumer
        {
            var services = configurator.Collection;

            services.AddSingleton<ConsumerTestHarnessRegistration<T>>();
            services.AddSingleton<IConsumerFactoryDecoratorRegistration<T>>(provider => provider.GetService<ConsumerTestHarnessRegistration<T>>());
            services.AddSingleton<IConsumerTestHarness<T>, RegistrationConsumerTestHarness<T>>();
        }

        /// <summary>
        /// Add a saga test harness for the specified saga to the container. The saga must be added separately, including
        /// a valid saga repository.
        /// </summary>
        public static void AddSagaTestHarness<T>(this IServiceCollectionBusConfigurator configurator)
            where T : class, ISaga
        {
            var services = configurator.Collection;

            services.AddSingleton<SagaTestHarnessRegistration<T>>();
            services.AddSingleton<ISagaRepositoryDecoratorRegistration<T>>(provider => provider.GetService<SagaTestHarnessRegistration<T>>());
            services.AddSingleton<ISagaTestHarness<T>, RegistrationSagaTestHarness<T>>();
        }

        /// <summary>
        /// Add a saga test harness for the specified saga to the container. The saga must be added separately, including
        /// a valid saga repository.
        /// </summary>
        public static void AddSagaStateMachineTestHarness<TStateMachine, TInstance>(this IServiceCollectionBusConfigurator configurator)
            where TInstance : class, SagaStateMachineInstance
            where TStateMachine : SagaStateMachine<TInstance>
        {
            var services = configurator.Collection;

            services.AddSingleton<SagaTestHarnessRegistration<TInstance>>();
            services.AddSingleton<ISagaRepositoryDecoratorRegistration<TInstance>>(provider => provider.GetService<SagaTestHarnessRegistration<TInstance>>());
            services.AddSingleton<IStateMachineSagaTestHarness<TInstance, TStateMachine>, RegistrationStateMachineSagaTestHarness<TInstance, TStateMachine>>();
        }
    }
}
