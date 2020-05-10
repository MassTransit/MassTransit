namespace MassTransit
{
    using System;
    using Automatonymous;
    using ConsumeConfigurators;
    using Courier;
    using Definition;
    using Saga;


    public interface IRegistrationConfigurator
    {
        /// <summary>
        /// Adds the consumer, allowing configuration when it is configured on an endpoint
        /// </summary>
        /// <param name="configure"></param>
        /// <typeparam name="T">The consumer type</typeparam>
        IConsumerRegistrationConfigurator<T> AddConsumer<T>(Action<IConsumerConfigurator<T>> configure = null)
            where T : class, IConsumer;

        /// <summary>
        /// Adds the consumer, allowing configuration when it is configured on an endpoint
        /// </summary>
        /// <param name="consumerDefinitionType">The consumer definition type</param>
        /// <param name="configure"></param>
        /// <typeparam name="T">The consumer type</typeparam>
        IConsumerRegistrationConfigurator<T> AddConsumer<T>(Type consumerDefinitionType, Action<IConsumerConfigurator<T>> configure = null)
            where T : class, IConsumer;

        /// <summary>
        /// Adds the consumer, along with an optional consumer definition
        /// </summary>
        /// <param name="consumerType">The consumer type</param>
        /// <param name="consumerDefinitionType">The consumer definition type</param>
        void AddConsumer(Type consumerType, Type consumerDefinitionType = null);

        /// <summary>
        /// Adds the saga, allowing configuration when it is configured on the endpoint. This should not
        /// be used for state machine (Automatonymous) sagas.
        /// </summary>
        /// <param name="configure"></param>
        /// <typeparam name="T">The saga type</typeparam>
        ISagaRegistrationConfigurator<T> AddSaga<T>(Action<ISagaConfigurator<T>> configure = null)
            where T : class, ISaga;

        /// <summary>
        /// Adds the saga, allowing configuration when it is configured on the endpoint. This should not
        /// be used for state machine (Automatonymous) sagas.
        /// </summary>
        /// <param name="sagaDefinitionType">The saga definition type</param>
        /// <param name="configure"></param>
        /// <typeparam name="T">The saga type</typeparam>
        ISagaRegistrationConfigurator<T> AddSaga<T>(Type sagaDefinitionType, Action<ISagaConfigurator<T>> configure = null)
            where T : class, ISaga;

        /// <summary>
        /// Adds the saga, along with an optional saga definition
        /// </summary>
        /// <param name="sagaType">The saga type</param>
        /// <param name="sagaDefinitionType">The saga definition type</param>
        void AddSaga(Type sagaType, Type sagaDefinitionType = null);

        /// <summary>
        /// Adds a SagaStateMachine to the registry, using the factory method, and updates the registrar prior to registering so that the default
        /// saga registrar isn't notified.
        /// </summary>
        /// <param name="configure"></param>
        /// <typeparam name="TStateMachine"></typeparam>
        /// <typeparam name="T"></typeparam>
        ISagaRegistrationConfigurator<T> AddSagaStateMachine<TStateMachine, T>(Action<ISagaConfigurator<T>> configure = null)
            where TStateMachine : class, SagaStateMachine<T>
            where T : class, SagaStateMachineInstance;

        /// <summary>
        /// Adds a SagaStateMachine to the registry, using the factory method, and updates the registrar prior to registering so that the default
        /// saga registrar isn't notified.
        /// </summary>
        /// <param name="sagaDefinitionType"></param>
        /// <param name="configure"></param>
        /// <typeparam name="TStateMachine"></typeparam>
        /// <typeparam name="T"></typeparam>
        ISagaRegistrationConfigurator<T> AddSagaStateMachine<TStateMachine, T>(Type sagaDefinitionType, Action<ISagaConfigurator<T>> configure = null)
            where TStateMachine : class, SagaStateMachine<T>
            where T : class, SagaStateMachineInstance;

        /// <summary>
        /// Adds the state machine saga, along with an optional saga definition
        /// </summary>
        /// <param name="sagaType">The saga type</param>
        /// <param name="sagaDefinitionType">The saga definition type</param>
        void AddSagaStateMachine(Type sagaType, Type sagaDefinitionType = null);

        /// <summary>
        /// Adds an execute activity (Courier), allowing configuration when it is configured on the endpoint.
        /// </summary>
        /// <param name="configure"></param>
        /// <typeparam name="TActivity">The activity type</typeparam>
        /// <typeparam name="TArguments">The argument type</typeparam>
        IExecuteActivityRegistrationConfigurator<TActivity, TArguments> AddExecuteActivity<TActivity, TArguments>(
            Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure = null)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class;

        /// <summary>
        /// Adds an execute activity (Courier), allowing configuration when it is configured on the endpoint.
        /// </summary>
        /// <param name="executeActivityDefinitionType"></param>
        /// <param name="configure"></param>
        /// <typeparam name="TActivity">The activity type</typeparam>
        /// <typeparam name="TArguments">The argument type</typeparam>
        IExecuteActivityRegistrationConfigurator<TActivity, TArguments> AddExecuteActivity<TActivity, TArguments>(Type executeActivityDefinitionType,
            Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure = null)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class;

        /// <summary>
        /// Adds an execute activity (Courier), along with an optional activity definition
        /// </summary>
        /// <param name="activityType"></param>
        /// <param name="activityDefinitionType"></param>
        void AddExecuteActivity(Type activityType, Type activityDefinitionType);

        /// <summary>
        /// Adds an activity (Courier), allowing configuration when it is configured on the endpoint.
        /// </summary>
        /// <param name="configureExecute">The execute configuration callback</param>
        /// <param name="configureCompensate">The compensate configuration callback</param>
        /// <typeparam name="TActivity">The activity type</typeparam>
        /// <typeparam name="TArguments">The argument type</typeparam>
        /// <typeparam name="TLog">The log type</typeparam>
        IActivityRegistrationConfigurator<TActivity, TArguments, TLog> AddActivity<TActivity, TArguments, TLog>(
            Action<IExecuteActivityConfigurator<TActivity, TArguments>> configureExecute = null,
            Action<ICompensateActivityConfigurator<TActivity, TLog>> configureCompensate = null)
            where TActivity : class, IActivity<TArguments, TLog>
            where TLog : class
            where TArguments : class;

        /// <summary>
        /// Adds an activity (Courier), allowing configuration when it is configured on the endpoint.
        /// </summary>
        /// <param name="activityDefinitionType"></param>
        /// <param name="configureExecute">The execute configuration callback</param>
        /// <param name="configureCompensate">The compensate configuration callback</param>
        /// <typeparam name="TActivity">The activity type</typeparam>
        /// <typeparam name="TArguments">The argument type</typeparam>
        /// <typeparam name="TLog">The log type</typeparam>
        IActivityRegistrationConfigurator<TActivity, TArguments, TLog> AddActivity<TActivity, TArguments, TLog>(Type activityDefinitionType,
            Action<IExecuteActivityConfigurator<TActivity, TArguments>> configureExecute = null,
            Action<ICompensateActivityConfigurator<TActivity, TLog>> configureCompensate = null)
            where TActivity : class, IActivity<TArguments, TLog>
            where TLog : class
            where TArguments : class;

        /// <summary>
        /// Adds an activity (Courier), along with an optional activity definition
        /// </summary>
        /// <param name="activityType"></param>
        /// <param name="activityDefinitionType"></param>
        void AddActivity(Type activityType, Type activityDefinitionType = null);

        /// <summary>
        /// Adds an endpoint definition, which will to used for consumers, sagas, etc. that are on that same endpoint. If a consumer, etc.
        /// specifies an endpoint without a definition, the default endpoint definition is used if one cannot be resolved from the configuration
        /// service provider (via generic registration).
        /// </summary>
        /// <param name="endpointDefinition">The endpoint definition to add</param>
        void AddEndpoint(Type endpointDefinition);

        void AddEndpoint<TDefinition, T>(IEndpointSettings<IEndpointDefinition<T>> settings = null)
            where TDefinition : class, IEndpointDefinition<T>
            where T : class;

        /// <summary>
        /// Add a request client, for the request type, which uses the <see cref="ConsumeContext"/> if present, otherwise
        /// uses the <see cref="IBus"/>. The request is published, unless an endpoint convention is specified for the
        /// request type.
        /// </summary>
        /// <param name="timeout">The request timeout</param>
        /// <typeparam name="T">The request message type</typeparam>
        void AddRequestClient<T>(RequestTimeout timeout = default)
            where T : class;

        /// <summary>
        /// Add a request client, for the request type, which uses the <see cref="ConsumeContext"/> if present, otherwise
        /// uses the <see cref="IBus"/>.
        /// </summary>
        /// <param name="destinationAddress">The destination address for the request</param>
        /// <param name="timeout">The request timeout</param>
        /// <typeparam name="T">The request message type</typeparam>
        void AddRequestClient<T>(Uri destinationAddress, RequestTimeout timeout = default)
            where T : class;

        /// <summary>
        /// Adds the service client, which enables service discovery via Conductor
        /// </summary>
        /// <param name="configure">Configure the service client</param>
        void AddServiceClient(Action<IServiceClientConfigurator> configure = default);

        /// <summary>
        /// Set the default endpoint name formatter used for endpoint names
        /// </summary>
        /// <param name="endpointNameFormatter"></param>
        void SetEndpointNameFormatter(IEndpointNameFormatter endpointNameFormatter);
    }


    /// <summary>
    /// Configures the container registration, and supports creation of a bus or a mediator.
    /// </summary>
    /// <typeparam name="TContainerContext">The container context type (IServiceProvider, etc.)</typeparam>
    public interface IRegistrationConfigurator<out TContainerContext> :
        IRegistrationConfigurator
        where TContainerContext : class
    {
        /// <summary>
        /// Add the bus to the container, configured properly
        /// </summary>
        /// <param name="busFactory"></param>
        void AddBus(Func<IRegistrationContext<TContainerContext>, IBusControl> busFactory);

        /// <summary>
        /// Add a mediator to the container
        /// </summary>
        /// <param name="configure">Optionally configure the pipeline used by the mediator</param>
        void AddMediator(Action<TContainerContext, IReceiveEndpointConfigurator> configure = null);
    }


    /// <summary>
    /// Configures the container registration for an additional bus instance, with is an advanced concept
    /// </summary>
    /// <typeparam name="TBus"></typeparam>
    /// <typeparam name="TContainerContext"></typeparam>
    public interface IRegistrationConfigurator<in TBus, out TContainerContext> :
        IRegistrationConfigurator
        where TBus : class, IBus
        where TContainerContext : class
    {
        /// <summary>
        /// Add the bus to the container, configured properly
        /// </summary>
        /// <param name="busFactory"></param>
        void AddBus(Func<IRegistrationContext<TContainerContext>, IBusControl> busFactory);
    }
}
