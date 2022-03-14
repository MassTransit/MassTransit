namespace MassTransit.Configuration
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using DependencyInjection.Registration;
    using Internals;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;


    /// <summary>
    /// Used for registration of consumers and sagas
    /// </summary>
    public class RegistrationConfigurator :
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

        protected Func<IServiceProvider, IBus, IClientFactory> ClientFactoryProvider { get; } = BusClientFactoryProvider;

        public IConsumerRegistrationConfigurator<T> AddConsumer<T>(Action<IConsumerConfigurator<T>> configure)
            where T : class, IConsumer
        {
            return AddConsumer(null, configure);
        }

        public IConsumerRegistrationConfigurator<T> AddConsumer<T>(Type consumerDefinitionType, Action<IConsumerConfigurator<T>> configure = null)
            where T : class, IConsumer
        {
            var registration = _collection.RegisterConsumer<T>(Registrar, consumerDefinitionType);

            registration.AddConfigureAction(configure);

            return new ConsumerRegistrationConfigurator<T>(this);
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
                throw new ArgumentException($"State machine sagas must be registered using AddSagaStateMachine: {TypeCache<T>.ShortName}");

            var registration = _collection.RegisterSaga<T>(Registrar, sagaDefinitionType);

            registration.AddConfigureAction(configure);

            return new SagaRegistrationConfigurator<T>(this);
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
            var registration = _collection.RegisterSagaStateMachine<TStateMachine, T>(Registrar, sagaDefinitionType);

            registration.AddConfigureAction(configure);

            return new SagaRegistrationConfigurator<T>(this);
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
            var registration = _collection.RegisterExecuteActivity<TActivity, TArguments>(Registrar, executeActivityDefinitionType);

            registration.AddConfigureAction(configure);

            return new ExecuteActivityRegistrationConfigurator<TActivity, TArguments>(this);
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
            var registration = _collection.RegisterActivity<TActivity, TArguments, TLog>(Registrar, activityDefinitionType);

            registration.AddConfigureAction(configureExecute);
            registration.AddConfigureAction(configureCompensate);

            return new ActivityRegistrationConfigurator<TActivity, TArguments, TLog>(this);
        }

        public IFutureRegistrationConfigurator<TFuture> AddFuture<TFuture>(Type futureDefinitionType)
            where TFuture : class, SagaStateMachine<FutureState>
        {
            _collection.RegisterFuture<TFuture>(Registrar, futureDefinitionType);

            return new FutureRegistrationConfigurator<TFuture>(this);
        }

        public void AddConfigureEndpointsCallback(ConfigureEndpointsCallback callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            _collection.TryAddSingleton<IConfigureReceiveEndpoint>(provider => new ConfigureReceiveEndpointDelegate(callback));
        }

        public void AddConfigureEndpointsCallback(ConfigureEndpointsProviderCallback callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            _collection.TryAddSingleton<IConfigureReceiveEndpoint>(provider => new ConfigureReceiveEndpointDelegateProvider(provider, callback));
        }

        public void AddEndpoint(Type definitionType)
        {
            _collection.RegisterEndpoint(Registrar, definitionType);
        }

        public void AddEndpoint<TDefinition, T>(IEndpointSettings<IEndpointDefinition<T>> settings)
            where TDefinition : class, IEndpointDefinition<T>
            where T : class
        {
            _collection.RegisterEndpoint<TDefinition, T>(Registrar, settings);
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
            _collection.TryAddSingleton(endpointNameFormatter);
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

        public void Complete()
        {
            if (_sagaRepositoryRegistrationProvider != null)
            {
                List<ISagaRegistration> registrations = Registrar.GetRegistrations<ISagaRegistration>().ToList();

                foreach (var registration in registrations)
                {
                    if (_collection.Any(x => x.ServiceType == typeof(ISagaRepository<>).MakeGenericType(registration.Type)))
                        continue;

                    var register = (IConfigureSagaRepository)Activator.CreateInstance(typeof(ConfigureSagaRepository<>).MakeGenericType(registration.Type));

                    register.Configure(this, _sagaRepositoryRegistrationProvider);
                }
            }
        }

        protected IRegistrationContext CreateRegistration(IServiceProvider provider)
        {
            return new RegistrationContext(provider, Registrar);
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

        static IClientFactory BusClientFactoryProvider(IServiceProvider provider, IBus bus)
        {
            return bus.CreateClientFactory();
        }


        interface IConfigureSagaRepository
        {
            void Configure(IRegistrationConfigurator configurator, ISagaRepositoryRegistrationProvider provider);
        }


        class ConfigureSagaRepository<TSaga> :
            IConfigureSagaRepository
            where TSaga : class, ISaga
        {
            public void Configure(IRegistrationConfigurator configurator, ISagaRepositoryRegistrationProvider provider)
            {
                var registrationConfigurator = new SagaRegistrationConfigurator<TSaga>(configurator);

                provider.Configure(registrationConfigurator);
            }
        }
    }
}
