namespace MassTransit.Registration
{
    using System;
    using Automatonymous;
    using Configurators;
    using ConsumeConfigurators;
    using Context;
    using Courier;
    using Definition;
    using Futures;
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
        readonly RegistrationCache<IActivityRegistration> _activities;
        readonly RegistrationCache<IConsumerRegistration> _consumers;
        readonly RegistrationCache<IEndpointRegistration> _endpoints;
        readonly RegistrationCache<IExecuteActivityRegistration> _executeActivities;
        readonly RegistrationCache<IFutureRegistration> _futures;
        readonly RegistrationCache<ISagaRegistration> _sagas;
        bool _configured;
        ISagaRepositoryRegistrationProvider _sagaRepositoryRegistrationProvider;

        protected RegistrationConfigurator(IContainerRegistrar registrar = null)
        {
            Registrar = registrar ?? new NullContainerRegistrar();

            _consumers = new RegistrationCache<IConsumerRegistration>();
            _sagas = new RegistrationCache<ISagaRegistration>();
            _executeActivities = new RegistrationCache<IExecuteActivityRegistration>();
            _activities = new RegistrationCache<IActivityRegistration>();
            _futures = new RegistrationCache<IFutureRegistration>();
            _endpoints = new RegistrationCache<IEndpointRegistration>();

            _sagaRepositoryRegistrationProvider = new SagaRepositoryRegistrationProvider();
        }

        protected IRegistrationCache<IActivityRegistration> Activities => _activities;
        protected IRegistrationCache<IConsumerRegistration> Consumers => _consumers;
        protected IRegistrationCache<IEndpointRegistration> Endpoints => _endpoints;
        protected IRegistrationCache<IExecuteActivityRegistration> ExecuteActivities => _executeActivities;
        protected IRegistrationCache<ISagaRegistration> Sagas => _sagas;
        protected IRegistrationCache<IFutureRegistration> Futures => _futures;

        public IContainerRegistrar Registrar { get; }

        protected Func<IConfigurationServiceProvider, IBus, IClientFactory> ClientFactoryProvider { get; } = BusClientFactoryProvider;

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
                ConsumerRegistrationCache.Register(type, Registrar);

                return new ConsumerRegistration<T>();
            }

            var registration = _consumers.GetOrAdd(typeof(T), ValueFactory);

            registration.AddConfigureAction(configure);

            if (consumerDefinitionType != null)
                ConsumerDefinitionRegistrationCache.Register(consumerDefinitionType, Registrar);

            return new ConsumerRegistrationConfigurator<T>(this);
        }

        public IConsumerRegistrationConfigurator AddConsumer(Type consumerType, Type consumerDefinitionType)
        {
            if (TypeMetadataCache.HasSagaInterfaces(consumerType))
            {
                throw new ArgumentException($"{TypeMetadataCache.GetShortName(consumerType)} is a saga, and cannot be registered as a consumer",
                    nameof(consumerType));
            }

            return ConsumerRegistrationCache.AddConsumer(this, consumerType, consumerDefinitionType);
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
                SagaRegistrationCache.Register(type, Registrar);

                return new SagaRegistration<T>();
            }

            var registration = _sagas.GetOrAdd(typeof(T), ValueFactory);

            registration.AddConfigureAction(configure);

            if (sagaDefinitionType != null)
                SagaDefinitionRegistrationCache.Register(sagaDefinitionType, Registrar);

            return new SagaRegistrationConfigurator<T>(this, Registrar);
        }

        public ISagaRegistrationConfigurator AddSaga(Type sagaType, Type sagaDefinitionType)
        {
            if (sagaType.HasInterface<SagaStateMachineInstance>())
                throw new ArgumentException($"State machine sagas must be registered using AddSagaStateMachine: {TypeMetadataCache.GetShortName(sagaType)}");

            return SagaRegistrationCache.AddSaga(this, _sagaRepositoryRegistrationProvider, sagaType, sagaDefinitionType);
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
                SagaStateMachineRegistrationCache.Register(typeof(TStateMachine), Registrar);

                return new SagaStateMachineRegistration<T>();
            }

            var registration = _sagas.GetOrAdd(typeof(T), ValueFactory);

            registration.AddConfigureAction(configure);

            if (sagaDefinitionType != null)
                SagaDefinitionRegistrationCache.Register(sagaDefinitionType, Registrar);

            return new SagaRegistrationConfigurator<T>(this, Registrar);
        }

        public void AddSagaStateMachine(Type sagaType, Type sagaDefinitionType)
        {
            SagaStateMachineRegistrationCache.AddSagaStateMachine(this, sagaType, _sagaRepositoryRegistrationProvider, sagaDefinitionType);
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
                ExecuteActivityRegistrationCache.Register(type, Registrar);

                return new ExecuteActivityRegistration<TActivity, TArguments>();
            }

            var registration = _executeActivities.GetOrAdd(typeof(TActivity), ValueFactory);

            registration.AddConfigureAction(configure);

            if (executeActivityDefinitionType != null)
                ExecuteActivityDefinitionRegistrationCache.Register(executeActivityDefinitionType, Registrar);

            return new ExecuteActivityRegistrationConfigurator<TActivity, TArguments>(this);
        }

        public IExecuteActivityRegistrationConfigurator AddExecuteActivity(Type activityType, Type activityDefinitionType)
        {
            return ExecuteActivityRegistrationCache.AddExecuteActivity(this, activityType, activityDefinitionType);
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
                ActivityRegistrationCache.Register(type, Registrar);

                return new ActivityRegistration<TActivity, TArguments, TLog>();
            }

            var registration = _activities.GetOrAdd(typeof(TActivity), ValueFactory);

            registration.AddConfigureAction(configureExecute);
            registration.AddConfigureAction(configureCompensate);

            if (activityDefinitionType != null)
                ActivityDefinitionRegistrationCache.Register(activityDefinitionType, Registrar);

            return new ActivityRegistrationConfigurator<TActivity, TArguments, TLog>(this);
        }

        public IActivityRegistrationConfigurator AddActivity(Type activityType, Type activityDefinitionType)
        {
            return ActivityRegistrationCache.AddActivity(this, activityType, activityDefinitionType);
        }

        public IFutureRegistrationConfigurator<TFuture> AddFuture<TFuture>(Type futureDefinitionType)
            where TFuture : MassTransitStateMachine<FutureState>
        {
            IFutureRegistration ValueFactory(Type type)
            {
                FutureRegistrationCache.Register(typeof(TFuture), Registrar);

                return new FutureRegistration<TFuture>();
            }

            _futures.GetOrAdd(typeof(TFuture), ValueFactory);

            if (futureDefinitionType != null)
                FutureDefinitionRegistrationCache.Register(futureDefinitionType, Registrar);

            return new FutureRegistrationConfigurator<TFuture>(this, Registrar);
        }

        public IFutureRegistrationConfigurator AddFuture(Type futureType, Type futureDefinitionType)
        {
            return FutureRegistrationCache.AddFuture(this, futureType, futureDefinitionType);
        }

        public void AddConfigureEndpointsCallback(ConfigureEndpointsCallback callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            Registrar.RegisterSingleInstance<IConfigureReceiveEndpoint>(provider => new DelegateConfigureReceiveEndpoint(callback));
        }

        public void AddEndpoint(Type definitionType)
        {
            _endpoints.GetOrAdd(definitionType, type => EndpointRegistrationCache.CreateRegistration(definitionType, Registrar));
        }

        public void AddEndpoint<TDefinition, T>(IEndpointSettings<IEndpointDefinition<T>> settings)
            where TDefinition : class, IEndpointDefinition<T>
            where T : class
        {
            IEndpointRegistration ValueFactory(Type type)
            {
                Registrar.RegisterEndpointDefinition<TDefinition, T>(settings);

                return new EndpointRegistration<T>();
            }

            _endpoints.GetOrAdd(typeof(TDefinition), ValueFactory);
        }

        public void AddRequestClient<T>(RequestTimeout timeout)
            where T : class
        {
            Registrar.RegisterRequestClient<T>(timeout);
        }

        public void AddRequestClient<T>(Uri destinationAddress, RequestTimeout timeout)
            where T : class
        {
            Registrar.RegisterRequestClient<T>(destinationAddress, timeout);
        }

        public void AddRequestClient(Type requestType, RequestTimeout timeout = default)
        {
            RequestClientRegistrationCache.Register(requestType, timeout, Registrar);
        }

        public void AddRequestClient(Type requestType, Uri destinationAddress, RequestTimeout timeout = default)
        {
            RequestClientRegistrationCache.Register(requestType, destinationAddress, timeout, Registrar);
        }

        public void SetEndpointNameFormatter(IEndpointNameFormatter endpointNameFormatter)
        {
            Registrar.RegisterSingleInstance(endpointNameFormatter);
        }

        public void AddMessageScheduler(IMessageSchedulerRegistration registration)
        {
            registration.Register(Registrar);
        }

        public ISagaRegistrationConfigurator<T> AddSagaRepository<T>()
            where T : class, ISaga
        {
            return new SagaRegistrationConfigurator<T>(this, Registrar);
        }

        public void SetSagaRepositoryProvider(ISagaRepositoryRegistrationProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            _sagaRepositoryRegistrationProvider = provider;
        }

        protected IRegistration CreateRegistration(IConfigurationServiceProvider provider)
        {
            return new Registration(provider, Consumers, Sagas, ExecuteActivities, Activities, Futures);
        }

        protected void ThrowIfAlreadyConfigured(string methodName)
        {
            if (_configured)
                throw new ConfigurationException($"'{methodName}' can be called only once.");

            _configured = true;
        }

        protected static void ConfigureLogContext(IConfigurationServiceProvider provider)
        {
            var loggerFactory = provider.GetService<ILoggerFactory>();
            if (loggerFactory != null)
                LogContext.ConfigureCurrentLogContext(loggerFactory);
            else if (LogContext.Current == null)
                LogContext.ConfigureCurrentLogContext();
        }

        static IClientFactory BusClientFactoryProvider(IConfigurationServiceProvider provider, IBus bus)
        {
            return bus.CreateClientFactory();
        }
    }
}
