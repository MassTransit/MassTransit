namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Configuration;
    using DependencyInjection.Registration;
    using DependencyInjection.Testing;
    using Logging;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using Testing;
    using Testing.Implementations;
    using Transports;


    public static class DependencyInjectionTestingExtensions
    {
        /// <summary>
        /// AddMassTransit, including the test harness, to the container.
        /// To specify a transport, add the appropriate UsingXxx method. If a transport is not specified, the
        /// default in-memory transport will be used, and ConfigureEndpoints will be called.
        /// </summary>
        public static IServiceCollection AddMassTransitTestHarness(this IServiceCollection services, Action<IBusRegistrationConfigurator> configure = null)
        {
            return AddMassTransitTestHarness(services, Console.Out, configure);
        }

        /// <summary>
        /// AddMassTransit, including the test harness, to the container.
        /// To specify a transport, add the appropriate UsingXxx method. If a transport is not specified, the
        /// default in-memory transport will be used, and ConfigureEndpoints will be called.
        /// </summary>
        public static IServiceCollection AddMassTransitTestHarness(this IServiceCollection services, TextWriter textWriter,
            Action<IBusRegistrationConfigurator> configure = null)
        {
            services.TryAddSingleton<ILoggerFactory>(provider => new TextWriterLoggerFactory(textWriter, true));
            services.TryAddSingleton(typeof(ILogger<>), typeof(Logger<>));

            services.AddBusObserver<ContainerTestHarnessBusObserver>();
            services.TryAddSingleton<ITestHarness>(provider => provider.GetService<ContainerTestHarness>());
            services.TryAddSingleton<ContainerTestHarness>();

            services.AddOptions<MassTransitHostOptions>().Configure(options =>
            {
                options.WaitUntilStarted = true;
            });

            return services.AddMassTransit(x =>
            {
                var harnessConfigurator = new TestHarnessRegistrationConfigurator(x);

                harnessConfigurator.SetInMemorySagaRepositoryProvider();

                configure?.Invoke(harnessConfigurator);

                if (harnessConfigurator.UseDefaultBusFactory)
                    harnessConfigurator.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
            });
        }

        /// <summary>
        /// Add the In-Memory test harness to the container, and configure it using the callback specified.
        /// </summary>
        public static IServiceCollection AddMassTransitInMemoryTestHarness(this IServiceCollection services,
            Action<IBusRegistrationConfigurator> configure = null)
        {
            services.AddMassTransit(cfg =>
            {
                configure?.Invoke(cfg);

                cfg.SetBusFactory(new InMemoryTestHarnessRegistrationBusFactory());
            });
            services.AddSingleton(provider =>
            {
                var busInstances = provider.GetService<IEnumerable<IBusInstance>>();
                if (busInstances == null)
                {
                    var busInstance = provider.GetService<IBusInstance>();
                    busInstances = new[] { busInstance };
                }

                if (busInstances == null)
                    throw new ConfigurationException("No bus instances found");

                var testHarnessBusInstance = busInstances.FirstOrDefault(x => x is InMemoryTestHarnessBusInstance);
                if (testHarnessBusInstance is InMemoryTestHarnessBusInstance testInstance)
                    return testInstance.Harness;

                throw new ConfigurationException("Test Harness configuration is invalid");
            });
            services.AddSingleton<BusTestHarness>(provider => provider.GetRequiredService<InMemoryTestHarness>());

            return services;
        }

        /// <summary>
        /// Add a consumer test harness for the specified consumer to the container
        /// </summary>
        [Obsolete("Consider migrating to AddMassTransitTestHarness, which does not require this extra configuration")]
        public static void AddConsumerTestHarness<T>(this IBusRegistrationConfigurator configurator)
            where T : class, IConsumer
        {
            configurator.AddSingleton<ConsumerTestHarnessRegistration<T>>();
            configurator.AddSingleton<IConsumerFactoryDecoratorRegistration<T>>(provider => provider.GetService<ConsumerTestHarnessRegistration<T>>());
            configurator.AddSingleton<IConsumerTestHarness<T>, RegistrationConsumerTestHarness<T>>();
        }

        /// <summary>
        /// Add a saga test harness for the specified saga to the container. The saga must be added separately, including
        /// a valid saga repository.
        /// </summary>
        [Obsolete("Consider migrating to AddMassTransitTestHarness, which does not require this extra configuration")]
        public static void AddSagaTestHarness<T>(this IBusRegistrationConfigurator configurator)
            where T : class, ISaga
        {
            configurator.AddSingleton<SagaTestHarnessRegistration<T>>();
            configurator.AddSingleton<ISagaRepositoryDecoratorRegistration<T>>(provider => provider.GetService<SagaTestHarnessRegistration<T>>());
            configurator.AddSingleton<ISagaTestHarness<T>, RegistrationSagaTestHarness<T>>();
        }

        /// <summary>
        /// Add a saga test harness for the specified saga to the container. The saga must be added separately, including
        /// a valid saga repository.
        /// </summary>
        [Obsolete("Consider migrating to AddMassTransitTestHarness, which does not require this extra configuration")]
        public static void AddSagaStateMachineTestHarness<TStateMachine, TSaga>(this IBusRegistrationConfigurator configurator)
            where TSaga : class, SagaStateMachineInstance
            where TStateMachine : SagaStateMachine<TSaga>
        {
            configurator.AddSingleton<SagaTestHarnessRegistration<TSaga>>();
            configurator.AddSingleton<ISagaRepositoryDecoratorRegistration<TSaga>>(provider => provider.GetService<SagaTestHarnessRegistration<TSaga>>());

            configurator.AddSingleton<RegistrationSagaStateMachineTestHarness<TStateMachine, TSaga>>();
            configurator.AddSingleton<ISagaStateMachineTestHarness<TStateMachine, TSaga>>(provider =>
                provider.GetService<RegistrationSagaStateMachineTestHarness<TStateMachine, TSaga>>());
        #pragma warning disable CS0618
            configurator.AddSingleton<IStateMachineSagaTestHarness<TSaga, TStateMachine>>(provider =>
            #pragma warning restore CS0618
                provider.GetService<RegistrationSagaStateMachineTestHarness<TStateMachine, TSaga>>());
        }
    }
}
