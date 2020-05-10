namespace MassTransit.Registration
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using Automatonymous;
    using ConsumeConfigurators;
    using Context;
    using Courier;
    using Definition;
    using Internals.Extensions;
    using Metadata;
    using Microsoft.Extensions.Logging;
    using Saga;


    /// <summary>
    /// Used for registration of consumers and sagas
    /// </summary>
    public class RegistrationConfigurator :
        IRegistrationConfigurator
    {
        readonly IContainerRegistrar _containerRegistrar;
        readonly ConcurrentDictionary<Type, IConsumerRegistration> _consumerRegistrations;
        readonly ConcurrentDictionary<Type, IExecuteActivityRegistration> _executeActivityRegistrations;
        readonly ConcurrentDictionary<Type, IActivityRegistration> _activityRegistrations;
        readonly ConcurrentDictionary<Type, ISagaRegistration> _sagaRegistrations;
        readonly ConcurrentDictionary<Type, IEndpointRegistration> _endpointRegistrations;
        bool _configured;

        protected Func<IConfigurationServiceProvider, IBus, IClientFactory> ClientFactoryProvider { get; private set; } = BusClientFactoryProvider;

        public RegistrationConfigurator(IContainerRegistrar containerRegistrar = null)
        {
            _containerRegistrar = containerRegistrar ?? new NullContainerRegistrar();

            _consumerRegistrations = new ConcurrentDictionary<Type, IConsumerRegistration>();
            _sagaRegistrations = new ConcurrentDictionary<Type, ISagaRegistration>();
            _executeActivityRegistrations = new ConcurrentDictionary<Type, IExecuteActivityRegistration>();
            _activityRegistrations = new ConcurrentDictionary<Type, IActivityRegistration>();
            _endpointRegistrations = new ConcurrentDictionary<Type, IEndpointRegistration>();
        }

        public IConsumerRegistrationConfigurator<T> AddConsumer<T>(Action<IConsumerConfigurator<T>> configure)
            where T : class, IConsumer
        {
            return AddConsumer(null, configure);
        }

        public IConsumerRegistrationConfigurator<T> AddConsumer<T>(Type consumerDefinitionType, Action<IConsumerConfigurator<T>> configure = null)
            where T : class, IConsumer
        {
            if (TypeMetadataCache<T>.HasSagaInterfaces)
                throw new ArgumentException($"{TypeMetadataCache<T>.ShortName} is a saga, and cannot be registered as a consumer", nameof(T));

            IConsumerRegistration ValueFactory(Type type)
            {
                ConsumerRegistrationCache.Register(type, _containerRegistrar);

                if (consumerDefinitionType != null)
                    ConsumerDefinitionRegistrationCache.Register(consumerDefinitionType, _containerRegistrar);

                return new ConsumerRegistration<T>();
            }

            var registration = _consumerRegistrations.GetOrAdd(typeof(T), ValueFactory);

            registration.AddConfigureAction(configure);

            return new ConsumerRegistrationConfigurator<T>(this);
        }

        public void AddConsumer(Type consumerType, Type consumerDefinitionType)
        {
            if (TypeMetadataCache.HasSagaInterfaces(consumerType))
                throw new ArgumentException($"{TypeMetadataCache.GetShortName(consumerType)} is a saga, and cannot be registered as a consumer",
                    nameof(consumerType));

            IConsumerRegistration ValueFactory(Type type)
            {
                ConsumerRegistrationCache.Register(type, _containerRegistrar);

                if (consumerDefinitionType != null)
                    ConsumerDefinitionRegistrationCache.Register(consumerDefinitionType, _containerRegistrar);

                return (IConsumerRegistration)Activator.CreateInstance(typeof(ConsumerRegistration<>).MakeGenericType(type));
            }

            _consumerRegistrations.GetOrAdd(consumerType, ValueFactory);
        }

        public ISagaRegistrationConfigurator<T> AddSaga<T>(Action<ISagaConfigurator<T>> configure)
            where T : class, ISaga
        {
            return AddSaga(null, configure);
        }

        public ISagaRegistrationConfigurator<T> AddSaga<T>(Type sagaDefinitionType, Action<ISagaConfigurator<T>> configure = null)
            where T : class, ISaga
        {
            if (typeof(T).HasInterface<SagaStateMachineInstance>())
                throw new ArgumentException($"State machine sagas must be registered using AddSagaStateMachine: {TypeMetadataCache<T>.ShortName}");

            ISagaRegistration ValueFactory(Type type)
            {
                SagaRegistrationCache.Register(type, _containerRegistrar);

                if (sagaDefinitionType != null)
                    SagaDefinitionRegistrationCache.Register(sagaDefinitionType, _containerRegistrar);

                return new SagaRegistration<T>();
            }

            var registration = _sagaRegistrations.GetOrAdd(typeof(T), ValueFactory);

            registration.AddConfigureAction(configure);

            return new SagaRegistrationConfigurator<T>(this, _containerRegistrar);
        }

        public void AddSaga(Type sagaType, Type sagaDefinitionType)
        {
            if (sagaType.HasInterface<SagaStateMachineInstance>())
                throw new ArgumentException($"State machine sagas must be registered using AddSagaStateMachine: {TypeMetadataCache.GetShortName(sagaType)}");

            _sagaRegistrations.GetOrAdd(sagaType, type => SagaRegistrationCache.CreateRegistration(type, sagaDefinitionType, _containerRegistrar));
        }

        public ISagaRegistrationConfigurator<T> AddSagaStateMachine<TStateMachine, T>(Action<ISagaConfigurator<T>> configure = null)
            where TStateMachine : class, SagaStateMachine<T>
            where T : class, SagaStateMachineInstance
        {
            return AddSagaStateMachine<TStateMachine, T>(null, configure);
        }

        public ISagaRegistrationConfigurator<T> AddSagaStateMachine<TStateMachine, T>(Type sagaDefinitionType, Action<ISagaConfigurator<T>> configure = null)
            where TStateMachine : class, SagaStateMachine<T>
            where T : class, SagaStateMachineInstance
        {
            ISagaRegistration ValueFactory(Type type)
            {
                SagaStateMachineRegistrationCache.Register(typeof(TStateMachine), _containerRegistrar);

                if (sagaDefinitionType != null)
                    SagaDefinitionRegistrationCache.Register(sagaDefinitionType, _containerRegistrar);

                return new SagaStateMachineRegistration<T>();
            }

            var registration = _sagaRegistrations.GetOrAdd(typeof(T), ValueFactory);

            registration.AddConfigureAction(configure);

            return new SagaRegistrationConfigurator<T>(this, _containerRegistrar);
        }

        public void AddSagaStateMachine(Type sagaType, Type sagaDefinitionType)
        {
            SagaStateMachineRegistrationCache.AddSagaStateMachine(this, sagaType, sagaDefinitionType);
        }

        public IExecuteActivityRegistrationConfigurator<TActivity, TArguments> AddExecuteActivity<TActivity, TArguments>(
            Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            return AddExecuteActivity(null, configure);
        }

        public IExecuteActivityRegistrationConfigurator<TActivity, TArguments> AddExecuteActivity<TActivity, TArguments>(Type executeActivityDefinitionType,
            Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure = null)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            IExecuteActivityRegistration ValueFactory(Type type)
            {
                ExecuteActivityRegistrationCache.Register(type, _containerRegistrar);

                if (executeActivityDefinitionType != null)
                    ExecuteActivityDefinitionRegistrationCache.Register(executeActivityDefinitionType, _containerRegistrar);

                return new ExecuteActivityRegistration<TActivity, TArguments>();
            }

            var registration = _executeActivityRegistrations.GetOrAdd(typeof(TActivity), ValueFactory);

            registration.AddConfigureAction(configure);

            return new ExecuteActivityRegistrationConfigurator<TActivity, TArguments>(this);
        }

        public void AddExecuteActivity(Type activityType, Type activityDefinitionType)
        {
            _executeActivityRegistrations.GetOrAdd(activityType,
                type => ExecuteActivityRegistrationCache.CreateRegistration(type, activityDefinitionType, _containerRegistrar));
        }

        public IActivityRegistrationConfigurator<TActivity, TArguments, TLog> AddActivity<TActivity, TArguments, TLog>(
            Action<IExecuteActivityConfigurator<TActivity, TArguments>> configureExecute,
            Action<ICompensateActivityConfigurator<TActivity, TLog>> configureCompensate)
            where TActivity : class, IActivity<TArguments, TLog>
            where TArguments : class
            where TLog : class
        {
            return AddActivity(null, configureExecute, configureCompensate);
        }

        public IActivityRegistrationConfigurator<TActivity, TArguments, TLog> AddActivity<TActivity, TArguments, TLog>(Type activityDefinitionType,
            Action<IExecuteActivityConfigurator<TActivity, TArguments>> configureExecute = null,
            Action<ICompensateActivityConfigurator<TActivity, TLog>> configureCompensate = null)
            where TActivity : class, IActivity<TArguments, TLog>
            where TArguments : class
            where TLog : class
        {
            IActivityRegistration ValueFactory(Type type)
            {
                ActivityRegistrationCache.Register(type, _containerRegistrar);

                if (activityDefinitionType != null)
                    ActivityDefinitionRegistrationCache.Register(activityDefinitionType, _containerRegistrar);

                return new ActivityRegistration<TActivity, TArguments, TLog>();
            }

            var registration = _activityRegistrations.GetOrAdd(typeof(TActivity), ValueFactory);

            registration.AddConfigureAction(configureExecute);
            registration.AddConfigureAction(configureCompensate);

            return new ActivityRegistrationConfigurator<TActivity, TArguments, TLog>(this);
        }

        public void AddActivity(Type activityType, Type activityDefinitionType)
        {
            _activityRegistrations.GetOrAdd(activityType,
                type => ActivityRegistrationCache.CreateRegistration(type, activityDefinitionType, _containerRegistrar));
        }

        public void AddEndpoint(Type definitionType)
        {
            _endpointRegistrations.GetOrAdd(definitionType, type => EndpointRegistrationCache.CreateRegistration(definitionType, _containerRegistrar));
        }

        public void AddEndpoint<TDefinition, T>(IEndpointSettings<IEndpointDefinition<T>> settings)
            where TDefinition : class, IEndpointDefinition<T>
            where T : class
        {
            IEndpointRegistration ValueFactory(Type type)
            {
                _containerRegistrar.RegisterEndpointDefinition<TDefinition, T>(settings);

                return new EndpointRegistration<T>();
            }

            _endpointRegistrations.GetOrAdd(typeof(TDefinition), ValueFactory);
        }

        public void AddRequestClient<T>(RequestTimeout timeout)
            where T : class
        {
            _containerRegistrar.RegisterRequestClient<T>(timeout);
        }

        public void AddRequestClient<T>(Uri destinationAddress, RequestTimeout timeout)
            where T : class
        {
            _containerRegistrar.RegisterRequestClient<T>(destinationAddress, timeout);
        }

        public void AddServiceClient(Action<IServiceClientConfigurator> configure = default)
        {
            var options = new ServiceClientOptions();

            _containerRegistrar.RegisterSingleInstance(options);

            ClientFactoryProvider = ServiceClientClientFactoryProvider;
        }

        public void SetEndpointNameFormatter(IEndpointNameFormatter endpointNameFormatter)
        {
            _containerRegistrar.RegisterSingleInstance(endpointNameFormatter);
        }

        public IRegistration CreateRegistration(IConfigurationServiceProvider configurationServiceProvider)
        {
            return new Registration(configurationServiceProvider, _consumerRegistrations.ToDictionary(x => x.Key, x => x.Value),
                _sagaRegistrations.ToDictionary(x => x.Key, x => x.Value), _executeActivityRegistrations.ToDictionary(x => x.Key, x => x.Value),
                _activityRegistrations.ToDictionary(x => x.Key, x => x.Value), _endpointRegistrations.ToDictionary(x => x.Key, x => x.Value));
        }

        protected void ThrowIfAlreadyConfigured()
        {
            if (_configured)
                throw new ConfigurationException("Either AddBus or AddMediator can be called, and only once.");

            _configured = true;
        }

        protected static void ConfigureLogContext(IConfigurationServiceProvider provider)
        {
            var loggerFactory = provider.GetService<ILoggerFactory>();
            if (loggerFactory != null)
                LogContext.ConfigureCurrentLogContext(loggerFactory);
        }

        protected void ConfigureMediator(IReceiveEndpointConfigurator configurator, IConfigurationServiceProvider provider)
        {
            var registration = new Registration(provider, _consumerRegistrations.ToDictionary(x => x.Key, x => x.Value),
                _sagaRegistrations.ToDictionary(x => x.Key, x => x.Value), _executeActivityRegistrations.ToDictionary(x => x.Key, x => x.Value),
                _activityRegistrations.ToDictionary(x => x.Key, x => x.Value), _endpointRegistrations.ToDictionary(x => x.Key, x => x.Value));

            registration.ConfigureConsumers(configurator);
            registration.ConfigureSagas(configurator);
        }

        static IClientFactory BusClientFactoryProvider(IConfigurationServiceProvider provider, IBus bus)
        {
            return bus.CreateClientFactory();
        }

        static IClientFactory ServiceClientClientFactoryProvider(IConfigurationServiceProvider provider, IBus bus)
        {
            return bus.CreateServiceClient();
        }
    }
}
