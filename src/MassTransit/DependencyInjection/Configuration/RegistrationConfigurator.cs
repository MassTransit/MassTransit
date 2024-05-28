namespace MassTransit.Configuration
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using DependencyInjection.Registration;
    using Internals;
    using Microsoft.Extensions.DependencyInjection;
    using Saga;


    /// <summary>
    /// Used for registration of consumers and sagas
    /// </summary>
    public abstract class RegistrationConfigurator :
        IRegistrationConfigurator
    {
        readonly IServiceCollection _collection;
        bool _configured;
        ISagaRepositoryRegistrationProvider _sagaRepositoryRegistrationProvider;

        protected RegistrationConfigurator(IServiceCollection collection, IContainerRegistrar registrar)
        {
            _collection = collection ?? throw new ArgumentNullException(nameof(collection));

            Registrar = registrar ?? new DependencyInjectionContainerRegistrar(collection);

            _sagaRepositoryRegistrationProvider = new SagaRepositoryRegistrationProvider();
        }

        public IContainerRegistrar Registrar { get; }

        protected RequestTimeout DefaultRequestTimeout { get; private set; } = RequestTimeout.Default;

        public IConsumerRegistrationConfigurator<T> AddConsumer<T>(Action<IRegistrationContext, IConsumerConfigurator<T>> configure = null)
            where T : class, IConsumer
        {
            return AddConsumer(null, configure);
        }

        public IConsumerRegistrationConfigurator<T> AddConsumer<T>(Type consumerDefinitionType,
            Action<IRegistrationContext, IConsumerConfigurator<T>> configure = null)
            where T : class, IConsumer
        {
            var registration = _collection.RegisterConsumer<T>(Registrar, consumerDefinitionType);

            registration.AddConfigureAction(configure);

            return new ConsumerRegistrationConfigurator<T>(this, registration);
        }

        public ISagaRegistrationConfigurator<T> AddSaga<T>(Action<IRegistrationContext, ISagaConfigurator<T>> configure)
            where T : class, ISaga
        {
            return AddSaga(null, configure);
        }

        public ISagaRegistrationConfigurator<T> AddSaga<T>(Type sagaDefinitionType, Action<IRegistrationContext, ISagaConfigurator<T>> configure = null)
            where T : class, ISaga
        {
            if (typeof(T).HasInterface<SagaStateMachineInstance>())
                throw new ArgumentException($"State machine sagas must be registered using AddSagaStateMachine: {TypeCache<T>.ShortName}");

            var registration = _collection.RegisterSaga<T>(Registrar, sagaDefinitionType);

            registration.AddConfigureAction(configure);

            return new SagaRegistrationConfigurator<T>(this, registration);
        }

        public ISagaRegistrationConfigurator<T> AddSagaStateMachine<TStateMachine, T>(Action<IRegistrationContext, ISagaConfigurator<T>> configure = null)
            where TStateMachine : class, SagaStateMachine<T>
            where T : class, SagaStateMachineInstance
        {
            return AddSagaStateMachine<TStateMachine, T>(null, configure);
        }

        public ISagaRegistrationConfigurator<T> AddSagaStateMachine<TStateMachine, T>(Type sagaDefinitionType,
            Action<IRegistrationContext, ISagaConfigurator<T>> configure = null)
            where TStateMachine : class, SagaStateMachine<T>
            where T : class, SagaStateMachineInstance
        {
            var registration = _collection.RegisterSagaStateMachine<TStateMachine, T>(Registrar, sagaDefinitionType);

            registration.AddConfigureAction(configure);

            return new SagaRegistrationConfigurator<T>(this, registration);
        }

        public IExecuteActivityRegistrationConfigurator<TActivity, TArguments> AddExecuteActivity<TActivity, TArguments>(
            Action<IRegistrationContext, IExecuteActivityConfigurator<TActivity, TArguments>> configure)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            return AddExecuteActivity(null, configure);
        }

        public IExecuteActivityRegistrationConfigurator<TActivity, TArguments> AddExecuteActivity<TActivity, TArguments>(Type executeActivityDefinitionType,
            Action<IRegistrationContext, IExecuteActivityConfigurator<TActivity, TArguments>> configure = null)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            var registration = _collection.RegisterExecuteActivity<TActivity, TArguments>(Registrar, executeActivityDefinitionType);

            registration.AddConfigureAction(configure);

            return new ExecuteActivityRegistrationConfigurator<TActivity, TArguments>(this, registration);
        }

        public IActivityRegistrationConfigurator<TActivity, TArguments, TLog> AddActivity<TActivity, TArguments, TLog>(
            Action<IRegistrationContext, IExecuteActivityConfigurator<TActivity, TArguments>> configureExecute,
            Action<IRegistrationContext, ICompensateActivityConfigurator<TActivity, TLog>> configureCompensate)
            where TActivity : class, IActivity<TArguments, TLog>
            where TArguments : class
            where TLog : class
        {
            return AddActivity(null, configureExecute, configureCompensate);
        }

        public IActivityRegistrationConfigurator<TActivity, TArguments, TLog> AddActivity<TActivity, TArguments, TLog>(Type activityDefinitionType,
            Action<IRegistrationContext, IExecuteActivityConfigurator<TActivity, TArguments>> configureExecute = null,
            Action<IRegistrationContext, ICompensateActivityConfigurator<TActivity, TLog>> configureCompensate = null)
            where TActivity : class, IActivity<TArguments, TLog>
            where TArguments : class
            where TLog : class
        {
            var registration = _collection.RegisterActivity<TActivity, TArguments, TLog>(Registrar, activityDefinitionType);

            registration.AddConfigureAction(configureExecute);
            registration.AddConfigureAction(configureCompensate);

            return new ActivityRegistrationConfigurator<TActivity, TArguments, TLog>(this, registration);
        }

        public IFutureRegistrationConfigurator<TFuture> AddFuture<TFuture>(Type futureDefinitionType)
            where TFuture : class, SagaStateMachine<FutureState>
        {
            var registration = _collection.RegisterFuture<TFuture>(Registrar, futureDefinitionType);

            return new FutureRegistrationConfigurator<TFuture>(this, registration);
        }

        public void AddEndpoint(Type definitionType)
        {
            _collection.RegisterEndpoint(Registrar, definitionType);
        }

        public void AddEndpoint<TDefinition, T>(IRegistration registration, IEndpointSettings<IEndpointDefinition<T>> settings)
            where TDefinition : class, IEndpointDefinition<T>
            where T : class
        {
            _collection.RegisterEndpoint<TDefinition, T>(Registrar, registration, settings);
        }

        public void AddRequestClient<T>(RequestTimeout timeout)
            where T : class
        {
            Registrar.RegisterRequestClient<T>(GetRequestTimeout(timeout));
        }

        public void AddRequestClient<T>(Uri destinationAddress, RequestTimeout timeout)
            where T : class
        {
            Registrar.RegisterRequestClient<T>(destinationAddress, timeout);
        }

        public void AddRequestClient(Type requestType, RequestTimeout timeout = default)
        {
            RequestClientRegistrationCache.Register(requestType, GetRequestTimeout(timeout), Registrar);
        }

        public void AddRequestClient(Type requestType, Uri destinationAddress, RequestTimeout timeout = default)
        {
            RequestClientRegistrationCache.Register(requestType, destinationAddress, GetRequestTimeout(timeout), Registrar);
        }

        public void SetDefaultRequestTimeout(RequestTimeout timeout)
        {
            DefaultRequestTimeout = timeout;
        }

        public void SetDefaultRequestTimeout(int? d = null, int? h = null, int? m = null, int? s = null, int? ms = null)
        {
            var timeout = new TimeSpan(d ?? 0, h ?? 0, m ?? 0, s ?? 0, ms ?? 0);
            if (timeout <= TimeSpan.Zero)
                throw new ArgumentException("The timeout must be > 0");

            DefaultRequestTimeout = timeout;
        }

        public void SetEndpointNameFormatter(IEndpointNameFormatter endpointNameFormatter)
        {
            Registrar.RegisterEndpointNameFormatter(endpointNameFormatter);
        }

        public ISagaRegistrationConfigurator<T> AddSagaRepository<T>()
            where T : class, ISaga
        {
            return new SagaRegistrationConfigurator<T>(this);
        }

        public void SetSagaRepositoryProvider(ISagaRepositoryRegistrationProvider provider)
        {
            _sagaRepositoryRegistrationProvider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public IEnumerator<ServiceDescriptor> GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_collection).GetEnumerator();
        }

        public void Add(ServiceDescriptor item)
        {
            _collection.Add(item);
        }

        public void Clear()
        {
            _collection.Clear();
        }

        public bool Contains(ServiceDescriptor item)
        {
            return _collection.Contains(item);
        }

        public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
        {
            _collection.CopyTo(array, arrayIndex);
        }

        public bool Remove(ServiceDescriptor item)
        {
            return _collection.Remove(item);
        }

        public int Count => _collection.Count;

        public bool IsReadOnly => _collection.IsReadOnly;

        public int IndexOf(ServiceDescriptor item)
        {
            return _collection.IndexOf(item);
        }

        public void Insert(int index, ServiceDescriptor item)
        {
            _collection.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _collection.RemoveAt(index);
        }

        public ServiceDescriptor this[int index]
        {
            get => _collection[index];
            set => _collection[index] = value;
        }

        RequestTimeout GetRequestTimeout(RequestTimeout timeout)
        {
            return timeout == RequestTimeout.Default ? DefaultRequestTimeout : timeout;
        }

        public void Complete()
        {
            if (_sagaRepositoryRegistrationProvider != null)
            {
                List<ISagaRegistration> registrations = Registrar.GetRegistrations<ISagaRegistration>().ToList();

                foreach (var registration in registrations)
                {
                    if (_collection.Any(x => x.ServiceType == typeof(ISagaRepositoryContextFactory<>).MakeGenericType(registration.Type)))
                        continue;

                    var register = (IConfigureSagaRepository)Activator.CreateInstance(typeof(ConfigureSagaRepository<>).MakeGenericType(registration.Type));

                    register.Configure(this, _sagaRepositoryRegistrationProvider, registration);
                }

                if (Registrar.GetRegistrations<IFutureRegistration>().Any()
                    && _collection.All(x => x.ServiceType != typeof(ISagaRepositoryContextFactory<FutureState>)))
                    new ConfigureSagaRepository<FutureState>().Configure(this, _sagaRepositoryRegistrationProvider, null);
            }
        }

        protected RegistrationContext CreateRegistration(IServiceProvider provider, ISetScopedConsumeContext setScopedConsumeContext)
        {
            return new RegistrationContext(provider, Registrar, setScopedConsumeContext);
        }

        protected void ThrowIfAlreadyConfigured(string methodName)
        {
            if (_configured)
                throw new ConfigurationException($"'{methodName}' can be called only once.");

            _configured = true;
        }

        protected static void ConfigureLogContext(IServiceProvider provider)
        {
            LogContext.ConfigureCurrentLogContextIfNull(provider);
        }


        interface IConfigureSagaRepository
        {
            void Configure(IRegistrationConfigurator configurator, ISagaRepositoryRegistrationProvider provider, ISagaRegistration registration);
        }


        class ConfigureSagaRepository<TSaga> :
            IConfigureSagaRepository
            where TSaga : class, ISaga
        {
            public void Configure(IRegistrationConfigurator configurator, ISagaRepositoryRegistrationProvider provider, ISagaRegistration registration)
            {
                var registrationConfigurator = new SagaRegistrationConfigurator<TSaga>(configurator, registration);

                provider.Configure(registrationConfigurator);
            }
        }
    }
}
