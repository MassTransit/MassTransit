#nullable enable
namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Configuration;
    using DependencyInjection;
    using DependencyInjection.Registration;
    using DependencyInjection.Testing;
    using Internals;
    using Logging;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Testing;
    using Testing.Implementations;
    using Transports;


    public static class DependencyInjectionTestingExtensions
    {
        /// <summary>
        /// AddMassTransit, including the test harness, to the container.
        /// To specify a transport, add the appropriate UsingXxx method. If a transport is not specified, the
        /// default in-memory transport will be used, and ConfigureEndpoints will be called.
        /// If MassTransit has already been configured, the existing bus configuration will be replaced with an in-memory
        /// configuration (by default, unless another UsingXxx transport method is specified), and saga repositories are
        /// replaced with in-memory as well.
        /// </summary>
        public static IServiceCollection AddMassTransitTestHarness(this IServiceCollection services, Action<IBusRegistrationConfigurator>? configure = null)
        {
            return AddMassTransitTestHarness(services, Console.Out, configure);
        }

        /// <summary>
        /// AddMassTransit, including the test harness, to the container.
        /// To specify a transport, add the appropriate UsingXxx method. If a transport is not specified, the
        /// default in-memory transport will be used, and ConfigureEndpoints will be called.
        /// If MassTransit has already been configured, the existing bus configuration will be replaced with an in-memory
        /// configuration (by default, unless another UsingXxx transport method is specified), and saga repositories are
        /// replaced with in-memory as well.
        /// </summary>
        public static IServiceCollection AddMassTransitTestHarness(this IServiceCollection services, TextWriter textWriter,
            Action<IBusRegistrationConfigurator>? configure = null)
        {
            AddMassTransitTextWriterLogger(services, textWriter);

            services.AddOptions<TestHarnessOptions>();
            services.AddBusObserver<ContainerTestHarnessBusObserver>();
            services.TryAddSingleton<ITestHarness>(provider => provider.GetRequiredService<ContainerTestHarness>());
            services.TryAddSingleton<ContainerTestHarness>();

            services.AddOptions<MassTransitHostOptions>().Configure(options =>
            {
                options.WaitUntilStarted = true;
            });

            services.TryAddSingleton<IValidateOptions<MassTransitHostOptions>, ValidateMassTransitHostOptions>();

            // If the bus was already configured, well, let's use it and any existing registrations
            if (services.Any(d => d.ServiceType == typeof(IBus)))
            {
                RegisterConsumerTestHarnesses(services);
                RegisterSagaTestHarnesses(services);

                services.RemoveMassTransit();
                services.RemoveSagaRepositories();
            }

            return services.AddMassTransit(x =>
            {
                var harnessConfigurator = new TestHarnessRegistrationConfigurator(x);

                harnessConfigurator.SetInMemorySagaRepositoryProvider();

                configure?.Invoke(harnessConfigurator);

                var addScheduler = false;
                if (services.All(d => d.ServiceType != typeof(IMessageScheduler)))
                {
                    x.AddDelayedMessageScheduler();
                    addScheduler = true;
                }

                if (harnessConfigurator.UseDefaultBusFactory)
                {
                    harnessConfigurator.UsingInMemory((context, cfg) =>
                    {
                        if (addScheduler)
                            cfg.UseDelayedMessageScheduler();

                        cfg.ConfigureEndpoints(context);
                    });
                }
            });
        }

        /// <summary>
        /// Internally used by AddMassTransitTestHarness to add a console-based <see cref="ILogger"/> for unit testing
        /// </summary>
        /// <param name="services"></param>
        /// <param name="textWriter"></param>
        public static IServiceCollection AddMassTransitTextWriterLogger(this IServiceCollection services, TextWriter? textWriter = null)
        {
            services.AddOptions<TextWriterLoggerOptions>();
            services.TryAddSingleton<ILoggerFactory>(provider =>
                new TextWriterLoggerFactory(textWriter ?? Console.Out, provider.GetRequiredService<IOptions<TextWriterLoggerOptions>>()));
            services.TryAddSingleton(typeof(ILogger<>), typeof(Logger<>));

            return services;
        }

        /// <summary>
        /// Adds a telemetry listener to the test harness, which outputs a timeline view of the unit test
        /// </summary>
        /// <param name="services"></param>
        /// <param name="includeDetails">If true, additional details from each span are shown</param>
        public static IServiceCollection AddTelemetryListener(this IServiceCollection services, bool includeDetails = false)
        {
            return services.AddTelemetryListener(Console.Out);
        }

        /// <summary>
        /// Adds a telemetry listener to the test harness, which outputs a timeline view of the unit test
        /// </summary>
        /// <param name="services"></param>
        /// <param name="textWriter">Override the default Console.Out TextWriter</param>
        /// <param name="includeDetails">If true, additional details from each span are shown</param>
        public static IServiceCollection AddTelemetryListener(this IServiceCollection services, TextWriter textWriter, bool includeDetails = false)
        {
            var (methodName, className) = GetTestMethodInfo();

            services.TryAddSingleton(provider => new TestActivityListener(textWriter, methodName, className, includeDetails));

            return services;
        }

        /// <summary>
        /// Specify the test and/or the test inactivity timeouts that should be used by the test harness.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="testTimeout">If specified, changes the test timeout</param>
        /// <param name="testInactivityTimeout">If specified, changes the test inactivity timeout</param>
        /// <returns></returns>
        public static IBusRegistrationConfigurator SetTestTimeouts(this IBusRegistrationConfigurator configurator, TimeSpan? testTimeout = default,
            TimeSpan? testInactivityTimeout = default)
        {
            configurator.AddOptions<TestHarnessOptions>()
                .Configure(options =>
                {
                    if (testTimeout.HasValue)
                        options.TestTimeout = testTimeout.Value;
                    if (testInactivityTimeout.HasValue)
                        options.TestInactivityTimeout = testInactivityTimeout.Value;
                });

            return configurator;
        }

        static void RegisterConsumerTestHarnesses(IServiceCollection services)
        {
            List<ServiceDescriptor> consumerRegistrations = services
                .Where(x => x.ServiceType == typeof(IConsumerRegistration))
                .ToList();

            foreach (var registration in consumerRegistrations)
            {
                if (registration.ImplementationInstance == null
                    || !registration.ImplementationInstance.GetType().ClosesType(typeof(ConsumerRegistration<>), out Type[] types))
                    continue;

                var type = typeof(RegistrationForConsumer<>).MakeGenericType(types[0]);
                var register = Activator.CreateInstance(type) as IRegisterTestHarness
                    ?? throw new InvalidOperationException("Could not create consumer registration");
                register.RegisterTestHarness(services);
            }
        }

        static void RegisterSagaTestHarnesses(IServiceCollection services)
        {
            List<ServiceDescriptor> sagaStateMachines = services
                .Where(x => x.ServiceType == typeof(ISagaRegistration))
                .ToList();

            foreach (var registration in sagaStateMachines)
            {
                if (registration.ImplementationInstance == null)
                    continue;

                if (registration.ImplementationInstance.GetType().ClosesType(typeof(SagaStateMachineRegistration<,>), out Type[] types))
                {
                    var type = typeof(RegistrationForSagaStateMachine<,>).MakeGenericType(types);
                    var register = Activator.CreateInstance(type) as IRegisterTestHarness
                        ?? throw new InvalidOperationException("Could not create consumer registration");
                    register.RegisterTestHarness(services);
                }
                else if (registration.ImplementationInstance.GetType().ClosesType(typeof(SagaRegistration<>), out types))
                {
                    var type = typeof(RegistrationForSaga<>).MakeGenericType(types);
                    var register = Activator.CreateInstance(type) as IRegisterTestHarness
                        ?? throw new InvalidOperationException("Could not create consumer registration");
                    register.RegisterTestHarness(services);
                }
            }
        }

        /// <summary>
        /// Add the In-Memory test harness to the container, and configure it using the callback specified.
        /// </summary>
        [Obsolete("Use AddMassTransitTestHarness instead. Visit https://masstransit.io/obsolete for details.")]
        public static IServiceCollection AddMassTransitInMemoryTestHarness(this IServiceCollection services,
            Action<IBusRegistrationConfigurator>? configure = null)
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
                    if (busInstance == null)
                        throw new ConfigurationException("No bus instances found");

                    busInstances = new[] { busInstance };
                }

                var testHarnessBusInstance = busInstances.FirstOrDefault(x => x is InMemoryTestHarnessBusInstance);
                if (testHarnessBusInstance is InMemoryTestHarnessBusInstance testInstance)
                    return testInstance.Harness;

                throw new ConfigurationException("Test Harness configuration is invalid");
            });
            services.AddSingleton<BusTestHarness>(provider => provider.GetRequiredService<InMemoryTestHarness>());

            return services;
        }

        static (string? methodName, string? className) GetTestMethodInfo()
        {
            var stackTrace = new StackTrace(2);
            var frameCount = stackTrace.FrameCount;
            for (var i = 0; i < frameCount; i++)
            {
                var frame = stackTrace.GetFrame(i);
                if (frame == null)
                    continue;

                var method = frame.GetMethod();
                if (method == null)
                    continue;

                if (method.GetCustomAttributes(false).Any(x =>
                    {
                        var name = x.GetType().Name;
                        return name.ToLower().Contains("test") || name.ToLower().Contains("fact");
                    }))
                    return (method.Name, method.DeclaringType?.Name);
            }

            return (null, null);
        }

        /// <summary>
        /// Add a consumer test harness for the specified consumer to the container
        /// </summary>
        public static void AddConsumerContainerTestHarness<T>(this IServiceCollection configurator)
            where T : class, IConsumer
        {
            configurator.TryAddSingleton<ConsumerContainerTestHarnessRegistration<T>>();
            configurator.TryAddSingleton<IConsumerFactoryDecoratorRegistration<T>>(provider =>
                provider.GetRequiredService<ConsumerContainerTestHarnessRegistration<T>>());
            configurator.TryAddSingleton<IConsumerTestHarness<T>, RegistrationConsumerTestHarness<T>>();
        }

        /// <summary>
        /// Add a saga test harness for the specified saga to the container. The saga must be added separately, including
        /// a valid saga repository.
        /// </summary>
        public static void AddSagaContainerTestHarness<T>(this IServiceCollection services)
            where T : class, ISaga
        {
            services.TryAddSingleton<SagaContainerTestHarnessRegistration<T>>();
            services.TryAddSingleton<ISagaRepositoryDecoratorRegistration<T>>(provider =>
                provider.GetRequiredService<SagaContainerTestHarnessRegistration<T>>());
            services.TryAddSingleton<ISagaTestHarness<T>, RegistrationSagaTestHarness<T>>();
        }

        /// <summary>
        /// Add a saga state machine test harness for the specified saga to the container. The saga must be added separately, including
        /// a valid saga repository.
        /// </summary>
        public static void AddSagaStateMachineContainerTestHarness<TStateMachine, T>(this IServiceCollection services)
            where TStateMachine : class, SagaStateMachine<T>
            where T : class, SagaStateMachineInstance
        {
            services.TryAddSingleton<SagaContainerTestHarnessRegistration<T>>();
            services.TryAddSingleton<ISagaRepositoryDecoratorRegistration<T>>(provider =>
                provider.GetRequiredService<SagaContainerTestHarnessRegistration<T>>());

            services.TryAddSingleton<RegistrationSagaStateMachineTestHarness<TStateMachine, T>>();
            services.TryAddSingleton<ISagaStateMachineTestHarness<TStateMachine, T>>(provider =>
                provider.GetRequiredService<RegistrationSagaStateMachineTestHarness<TStateMachine, T>>());
            #pragma warning disable CS0618
            services.TryAddSingleton<IStateMachineSagaTestHarness<T, TStateMachine>>(provider =>
                #pragma warning restore CS0618
                provider.GetRequiredService<RegistrationSagaStateMachineTestHarness<TStateMachine, T>>());
        }

        /// <summary>
        /// Add a consumer test harness for the specified consumer to the container
        /// </summary>
        [Obsolete("Use AddMassTransitTestHarness instead. Visit https://masstransit.io/obsolete for details.")]
        public static void AddConsumerTestHarness<T>(this IBusRegistrationConfigurator configurator)
            where T : class, IConsumer
        {
            configurator.AddSingleton<ConsumerTestHarnessRegistration<T>>();
            configurator.AddSingleton<IConsumerFactoryDecoratorRegistration<T>>(provider => provider.GetRequiredService<ConsumerTestHarnessRegistration<T>>());
            configurator.AddSingleton<IConsumerTestHarness<T>, RegistrationConsumerTestHarness<T>>();
        }

        /// <summary>
        /// Add a saga test harness for the specified saga to the container. The saga must be added separately, including
        /// a valid saga repository.
        /// </summary>
        [Obsolete("Use AddMassTransitTestHarness instead. Visit https://masstransit.io/obsolete for details.")]
        public static void AddSagaTestHarness<T>(this IBusRegistrationConfigurator configurator)
            where T : class, ISaga
        {
            configurator.AddSingleton<SagaTestHarnessRegistration<T>>();
            configurator.AddSingleton<ISagaRepositoryDecoratorRegistration<T>>(provider => provider.GetRequiredService<SagaTestHarnessRegistration<T>>());
            configurator.AddSingleton<ISagaTestHarness<T>, RegistrationSagaTestHarness<T>>();
        }

        /// <summary>
        /// Add a saga test harness for the specified saga to the container. The saga must be added separately, including
        /// a valid saga repository.
        /// </summary>
        [Obsolete("Use AddMassTransitTestHarness instead. Visit https://masstransit.io/obsolete for details.")]
        public static void AddSagaStateMachineTestHarness<TStateMachine, TSaga>(this IBusRegistrationConfigurator configurator)
            where TSaga : class, SagaStateMachineInstance
            where TStateMachine : SagaStateMachine<TSaga>
        {
            configurator.AddSingleton<SagaTestHarnessRegistration<TSaga>>();
            configurator.AddSingleton<ISagaRepositoryDecoratorRegistration<TSaga>>(provider =>
                provider.GetRequiredService<SagaTestHarnessRegistration<TSaga>>());

            configurator.AddSingleton<RegistrationSagaStateMachineTestHarness<TStateMachine, TSaga>>();
            configurator.AddSingleton<ISagaStateMachineTestHarness<TStateMachine, TSaga>>(provider =>
                provider.GetRequiredService<RegistrationSagaStateMachineTestHarness<TStateMachine, TSaga>>());
            #pragma warning disable CS0618
            configurator.AddSingleton<IStateMachineSagaTestHarness<TSaga, TStateMachine>>(provider =>
                #pragma warning restore CS0618
                provider.GetRequiredService<RegistrationSagaStateMachineTestHarness<TStateMachine, TSaga>>());
        }


        interface IRegisterTestHarness
        {
            void RegisterTestHarness(IServiceCollection services);
        }


        class RegistrationForConsumer<T> :
            IRegisterTestHarness
            where T : class, IConsumer
        {
            public void RegisterTestHarness(IServiceCollection services)
            {
                services.AddConsumerContainerTestHarness<T>();
            }
        }


        class RegistrationForSagaStateMachine<TStateMachine, TInstance> :
            IRegisterTestHarness
            where TStateMachine : class, SagaStateMachine<TInstance>
            where TInstance : class, SagaStateMachineInstance
        {
            public void RegisterTestHarness(IServiceCollection services)
            {
                services.RegisterInMemorySagaRepository<TInstance>();
                services.AddSagaStateMachineContainerTestHarness<TStateMachine, TInstance>();
            }
        }


        class RegistrationForSaga<TSaga> :
            IRegisterTestHarness
            where TSaga : class, ISaga
        {
            public void RegisterTestHarness(IServiceCollection services)
            {
                services.RegisterInMemorySagaRepository<TSaga>();
                services.AddSagaContainerTestHarness<TSaga>();
            }
        }
    }
}
