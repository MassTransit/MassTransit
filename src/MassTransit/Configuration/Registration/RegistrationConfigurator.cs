namespace MassTransit.Registration
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using Automatonymous;
    using ConsumeConfigurators;
    using Definition;
    using Internals.Extensions;
    using Metadata;


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

        public RegistrationConfigurator(IContainerRegistrar containerRegistrar = null)
        {
            _containerRegistrar = containerRegistrar ?? new NullContainerRegistrar();

            _consumerRegistrations = new ConcurrentDictionary<Type, IConsumerRegistration>();
            _sagaRegistrations = new ConcurrentDictionary<Type, ISagaRegistration>();
            _executeActivityRegistrations = new ConcurrentDictionary<Type, IExecuteActivityRegistration>();
            _activityRegistrations = new ConcurrentDictionary<Type, IActivityRegistration>();
            _endpointRegistrations = new ConcurrentDictionary<Type, IEndpointRegistration>();
        }

        IConsumerRegistrationConfigurator<T> IRegistrationConfigurator.AddConsumer<T>(Action<IConsumerConfigurator<T>> configure)
        {
            if (TypeMetadataCache<T>.HasSagaInterfaces)
                throw new ArgumentException($"{TypeMetadataCache<T>.ShortName} is a saga, and cannot be registered as a consumer", nameof(T));

            IConsumerRegistration ValueFactory(Type type)
            {
                ConsumerRegistrationCache.Register(type, _containerRegistrar);

                return new ConsumerRegistration<T>();
            }

            var registration = _consumerRegistrations.GetOrAdd(typeof(T), ValueFactory);

            registration.AddConfigureAction(configure);

            return new ConsumerRegistrationConfigurator<T>(this, registration, _containerRegistrar);
        }

        void IRegistrationConfigurator.AddConsumer(Type consumerType, Type consumerDefinitionType)
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

        ISagaRegistrationConfigurator<T> IRegistrationConfigurator.AddSaga<T>(Action<ISagaConfigurator<T>> configure)
        {
            if (typeof(T).HasInterface<SagaStateMachineInstance>())
                throw new ArgumentException($"State machine sagas must be registered using AddSagaStateMachine: {TypeMetadataCache<T>.ShortName}");

            ISagaRegistration ValueFactory(Type type)
            {
                SagaRegistrationCache.Register(type, _containerRegistrar);

                return new SagaRegistration<T>();
            }

            var registration = _sagaRegistrations.GetOrAdd(typeof(T), ValueFactory);

            registration.AddConfigureAction(configure);

            return new SagaRegistrationConfigurator<T>(this, registration, _containerRegistrar);
        }

        void IRegistrationConfigurator.AddSaga(Type sagaType, Type sagaDefinitionType)
        {
            if (sagaType.HasInterface<SagaStateMachineInstance>())
                throw new ArgumentException($"State machine sagas must be registered using AddSagaStateMachine: {TypeMetadataCache.GetShortName(sagaType)}");

            _sagaRegistrations.GetOrAdd(sagaType, type => SagaRegistrationCache.CreateRegistration(type, sagaDefinitionType, _containerRegistrar));
        }

        ISagaRegistrationConfigurator<T> IRegistrationConfigurator.AddSagaStateMachine<TStateMachine, T>(Type sagaDefinitionType)
        {
            ISagaRegistration Factory(IContainerRegistrar containerRegistrar)
            {
                SagaStateMachineRegistrationCache.Register(typeof(TStateMachine), containerRegistrar);

                if (sagaDefinitionType != null)
                    SagaDefinitionRegistrationCache.Register(sagaDefinitionType, containerRegistrar);

                return new SagaStateMachineRegistration<T>();
            }

            var registration = _sagaRegistrations.GetOrAdd(typeof(T), _ => Factory(_containerRegistrar));

            return new SagaRegistrationConfigurator<T>(this, registration, _containerRegistrar);
        }

        void IRegistrationConfigurator.AddSagaStateMachine(Type sagaType, Type sagaDefinitionType)
        {
            SagaStateMachineRegistrationCache.AddSagaStateMachine(this, sagaType, sagaDefinitionType);
        }

        IExecuteActivityRegistrationConfigurator<TActivity, TArguments> IRegistrationConfigurator.AddExecuteActivity<TActivity, TArguments>(
            Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure)
        {
            IExecuteActivityRegistration ValueFactory(Type type)
            {
                ExecuteActivityRegistrationCache.Register(type, _containerRegistrar);

                return new ExecuteActivityRegistration<TActivity, TArguments>();
            }

            var registration = _executeActivityRegistrations.GetOrAdd(typeof(TActivity), ValueFactory);

            registration.AddConfigureAction(configure);

            return new ExecuteActivityRegistrationConfigurator<TActivity, TArguments>(this, registration, _containerRegistrar);
        }

        void IRegistrationConfigurator.AddExecuteActivity(Type activityType, Type activityDefinitionType)
        {
            _executeActivityRegistrations.GetOrAdd(activityType,
                type => ExecuteActivityRegistrationCache.CreateRegistration(type, activityDefinitionType, _containerRegistrar));
        }

        IActivityRegistrationConfigurator<TActivity, TArguments, TLog> IRegistrationConfigurator.AddActivity<TActivity, TArguments, TLog>(
            Action<IExecuteActivityConfigurator<TActivity, TArguments>> configureExecute,
            Action<ICompensateActivityConfigurator<TActivity, TLog>> configureCompensate)
        {
            IActivityRegistration ValueFactory(Type type)
            {
                ActivityRegistrationCache.Register(type, _containerRegistrar);

                return new ActivityRegistration<TActivity, TArguments, TLog>();
            }

            var registration = _activityRegistrations.GetOrAdd(typeof(TActivity), ValueFactory);

            registration.AddConfigureAction(configureExecute);
            registration.AddConfigureAction(configureCompensate);

            return new ActivityRegistrationConfigurator<TActivity, TArguments, TLog>(this, registration, _containerRegistrar);
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

        void IRegistrationConfigurator.AddRequestClient<T>(RequestTimeout timeout)
        {
            _containerRegistrar.RegisterRequestClient<T>(timeout);
        }

        void IRegistrationConfigurator.AddRequestClient<T>(Uri destinationAddress, RequestTimeout timeout)
        {
            _containerRegistrar.RegisterRequestClient<T>(destinationAddress, timeout);
        }

        public IRegistration CreateRegistration(IConfigurationServiceProvider configurationServiceProvider)
        {
            return new Registration(configurationServiceProvider, _consumerRegistrations.ToDictionary(x => x.Key, x => x.Value),
                _sagaRegistrations.ToDictionary(x => x.Key, x => x.Value), _executeActivityRegistrations.ToDictionary(x => x.Key, x => x.Value),
                _activityRegistrations.ToDictionary(x => x.Key, x => x.Value), _endpointRegistrations.ToDictionary(x => x.Key, x => x.Value));
        }
    }
}
