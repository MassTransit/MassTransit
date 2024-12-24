namespace MassTransit.Configuration
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.Extensions.DependencyInjection;


    public class TestHarnessRegistrationConfigurator :
        IBusRegistrationConfigurator
    {
        readonly IBusRegistrationConfigurator _configurator;

        public TestHarnessRegistrationConfigurator(IBusRegistrationConfigurator configurator)
        {
            _configurator = configurator;
        }

        public bool UseDefaultBusFactory { get; private set; } = true;

        public IEnumerator<ServiceDescriptor> GetEnumerator()
        {
            return _configurator.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_configurator).GetEnumerator();
        }

        public void Add(ServiceDescriptor item)
        {
            _configurator.Add(item);
        }

        public void Clear()
        {
            _configurator.Clear();
        }

        public bool Contains(ServiceDescriptor item)
        {
            return _configurator.Contains(item);
        }

        public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
        {
            _configurator.CopyTo(array, arrayIndex);
        }

        public bool Remove(ServiceDescriptor item)
        {
            return _configurator.Remove(item);
        }

        public int Count => _configurator.Count;

        public bool IsReadOnly => _configurator.IsReadOnly;

        public int IndexOf(ServiceDescriptor item)
        {
            return _configurator.IndexOf(item);
        }

        public void Insert(int index, ServiceDescriptor item)
        {
            _configurator.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _configurator.RemoveAt(index);
        }

        public ServiceDescriptor this[int index]
        {
            get => _configurator[index];
            set => _configurator[index] = value;
        }

        public IConsumerRegistrationConfigurator<T> AddConsumer<T>(Action<IRegistrationContext, IConsumerConfigurator<T>> configure = null)
            where T : class, IConsumer
        {
            return AddConsumer(null, configure);
        }

        public IConsumerRegistrationConfigurator<T> AddConsumer<T>(Type consumerDefinitionType,
            Action<IRegistrationContext, IConsumerConfigurator<T>> configure = null)
            where T : class, IConsumer
        {
            IConsumerRegistrationConfigurator<T> registrationConfigurator = _configurator.AddConsumer(consumerDefinitionType, configure);

            _configurator.AddConsumerContainerTestHarness<T>();

            return registrationConfigurator;
        }

        public ISagaRegistrationConfigurator<T> AddSaga<T>(Action<IRegistrationContext, ISagaConfigurator<T>> configure = null)
            where T : class, ISaga
        {
            return AddSaga(null, configure);
        }

        public ISagaRegistrationConfigurator<T> AddSaga<T>(Type sagaDefinitionType, Action<IRegistrationContext, ISagaConfigurator<T>> configure = null)
            where T : class, ISaga
        {
            ISagaRegistrationConfigurator<T> registrationConfigurator = _configurator.AddSaga(sagaDefinitionType, configure);

            _configurator.AddSagaContainerTestHarness<T>();

            return registrationConfigurator;
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
            ISagaRegistrationConfigurator<T> registrationConfigurator = _configurator.AddSagaStateMachine<TStateMachine, T>(sagaDefinitionType, configure);

            _configurator.AddSagaStateMachineContainerTestHarness<TStateMachine, T>();

            return registrationConfigurator;
        }

        public IExecuteActivityRegistrationConfigurator<TActivity, TArguments> AddExecuteActivity<TActivity, TArguments>(
            Action<IRegistrationContext, IExecuteActivityConfigurator<TActivity, TArguments>> configure = null)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            return _configurator.AddExecuteActivity(configure);
        }

        public IExecuteActivityRegistrationConfigurator<TActivity, TArguments> AddExecuteActivity<TActivity, TArguments>(Type executeActivityDefinitionType,
            Action<IRegistrationContext, IExecuteActivityConfigurator<TActivity, TArguments>> configure = null)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            return _configurator.AddExecuteActivity(executeActivityDefinitionType, configure);
        }

        public IActivityRegistrationConfigurator<TActivity, TArguments, TLog> AddActivity<TActivity, TArguments, TLog>(
            Action<IRegistrationContext, IExecuteActivityConfigurator<TActivity, TArguments>> configureExecute = null,
            Action<IRegistrationContext, ICompensateActivityConfigurator<TActivity, TLog>> configureCompensate = null)
            where TActivity : class, IActivity<TArguments, TLog>
            where TArguments : class
            where TLog : class
        {
            return _configurator.AddActivity(configureExecute, configureCompensate);
        }

        public IActivityRegistrationConfigurator<TActivity, TArguments, TLog> AddActivity<TActivity, TArguments, TLog>(Type activityDefinitionType,
            Action<IRegistrationContext, IExecuteActivityConfigurator<TActivity, TArguments>> configureExecute = null,
            Action<IRegistrationContext, ICompensateActivityConfigurator<TActivity, TLog>> configureCompensate = null)
            where TActivity : class, IActivity<TArguments, TLog>
            where TArguments : class
            where TLog : class
        {
            return _configurator.AddActivity(activityDefinitionType, configureExecute, configureCompensate);
        }

        public void AddEndpoint(Type endpointDefinition)
        {
            _configurator.AddEndpoint(endpointDefinition);
        }

        public void AddEndpoint<TDefinition, T>(IRegistration registration, IEndpointSettings<IEndpointDefinition<T>> settings = null)
            where TDefinition : class, IEndpointDefinition<T>
            where T : class
        {
            _configurator.AddEndpoint<TDefinition, T>(registration, settings);
        }

        public void AddRequestClient<T>(RequestTimeout timeout = default)
            where T : class
        {
            _configurator.AddRequestClient<T>(timeout);
        }

        public void AddRequestClient<T>(Uri destinationAddress, RequestTimeout timeout = default)
            where T : class
        {
            _configurator.AddRequestClient<T>(destinationAddress, timeout);
        }

        public void AddRequestClient(Type requestType, RequestTimeout timeout = default)
        {
            _configurator.AddRequestClient(requestType, timeout);
        }

        public void AddRequestClient(Type requestType, Uri destinationAddress, RequestTimeout timeout = default)
        {
            _configurator.AddRequestClient(requestType, destinationAddress, timeout);
        }

        public void SetDefaultRequestTimeout(RequestTimeout timeout)
        {
            _configurator.SetDefaultRequestTimeout(timeout);
        }

        public void SetDefaultRequestTimeout(int? d = null, int? h = null, int? m = null, int? s = null, int? ms = null)
        {
            _configurator.SetDefaultRequestTimeout(d, h, m, s, ms);
        }

        public void SetEndpointNameFormatter(IEndpointNameFormatter endpointNameFormatter)
        {
            _configurator.SetEndpointNameFormatter(endpointNameFormatter);
        }

        public ISagaRegistrationConfigurator<T> AddSagaRepository<T>()
            where T : class, ISaga
        {
            return _configurator.AddSagaRepository<T>();
        }

        public void SetSagaRepositoryProvider(ISagaRepositoryRegistrationProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            _configurator.SetSagaRepositoryProvider(provider);
        }

        public IFutureRegistrationConfigurator<TFuture> AddFuture<TFuture>(Type futureDefinitionType = null)
            where TFuture : class, SagaStateMachine<FutureState>
        {
            return _configurator.AddFuture<TFuture>(futureDefinitionType);
        }

        public void AddConfigureEndpointsCallback(ConfigureEndpointsCallback callback)
        {
            _configurator.AddConfigureEndpointsCallback(callback);
        }

        public void AddConfigureEndpointsCallback(ConfigureEndpointsProviderCallback callback)
        {
            _configurator.AddConfigureEndpointsCallback(callback);
        }

        public void SetRequestClientFactory(Func<IBus, RequestTimeout, IClientFactory> clientFactory)
        {
            _configurator.SetRequestClientFactory(clientFactory);
        }

        public IContainerRegistrar Registrar => _configurator.Registrar;

        [Obsolete("Use 'Using[TransportName]' instead. Visit https://masstransit.io/obsolete for details.")]
        public void AddBus(Func<IBusRegistrationContext, IBusControl> busFactory)
        {
            _configurator.AddBus(busFactory);

            UseDefaultBusFactory = false;
        }

        public void SetBusFactory<T>(T busFactory)
            where T : class, IRegistrationBusFactory
        {
            _configurator.SetBusFactory(busFactory);

            UseDefaultBusFactory = false;
        }

        public void AddRider(Action<IRiderRegistrationConfigurator> configure)
        {
            _configurator.AddRider(configure);
        }
    }
}
