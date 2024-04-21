namespace MassTransit
{
    using System;
    using Configuration;
    using Microsoft.Extensions.DependencyInjection;


    public interface IRegistrationConfigurator :
        IServiceCollection
    {
        /// <summary>
        /// Adds the consumer, allowing configuration when it is configured on an endpoint
        /// </summary>
        /// <param name="configure"></param>
        /// <typeparam name="T">The consumer type</typeparam>
        IConsumerRegistrationConfigurator<T> AddConsumer<T>(Action<IRegistrationContext, IConsumerConfigurator<T>> configure = null)
            where T : class, IConsumer;

        /// <summary>
        /// Adds the consumer, allowing configuration when it is configured on an endpoint
        /// </summary>
        /// <param name="consumerDefinitionType">The consumer definition type</param>
        /// <param name="configure"></param>
        /// <typeparam name="T">The consumer type</typeparam>
        IConsumerRegistrationConfigurator<T> AddConsumer<T>(Type consumerDefinitionType,
            Action<IRegistrationContext, IConsumerConfigurator<T>> configure = null)
            where T : class, IConsumer;

        /// <summary>
        /// Adds the saga, allowing configuration when it is configured on the endpoint. This should not
        /// be used for state machine (Automatonymous) sagas.
        /// </summary>
        /// <param name="configure"></param>
        /// <typeparam name="T">The saga type</typeparam>
        ISagaRegistrationConfigurator<T> AddSaga<T>(Action<IRegistrationContext, ISagaConfigurator<T>> configure = null)
            where T : class, ISaga;

        /// <summary>
        /// Adds the saga, allowing configuration when it is configured on the endpoint. This should not
        /// be used for state machine (Automatonymous) sagas.
        /// </summary>
        /// <param name="sagaDefinitionType">The saga definition type</param>
        /// <param name="configure"></param>
        /// <typeparam name="T">The saga type</typeparam>
        ISagaRegistrationConfigurator<T> AddSaga<T>(Type sagaDefinitionType, Action<IRegistrationContext, ISagaConfigurator<T>> configure = null)
            where T : class, ISaga;

        /// <summary>
        /// Adds a SagaStateMachine to the registry, using the factory method, and updates the registrar prior to registering so that the default
        /// saga registrar isn't notified.
        /// </summary>
        /// <param name="configure"></param>
        /// <typeparam name="TStateMachine"></typeparam>
        /// <typeparam name="T"></typeparam>
        ISagaRegistrationConfigurator<T> AddSagaStateMachine<TStateMachine, T>(Action<IRegistrationContext, ISagaConfigurator<T>> configure = null)
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
        ISagaRegistrationConfigurator<T> AddSagaStateMachine<TStateMachine, T>(Type sagaDefinitionType,
            Action<IRegistrationContext, ISagaConfigurator<T>> configure = null)
            where TStateMachine : class, SagaStateMachine<T>
            where T : class, SagaStateMachineInstance;

        /// <summary>
        /// Adds an execute activity (Courier), allowing configuration when it is configured on the endpoint.
        /// </summary>
        /// <param name="configure"></param>
        /// <typeparam name="TActivity">The activity type</typeparam>
        /// <typeparam name="TArguments">The argument type</typeparam>
        IExecuteActivityRegistrationConfigurator<TActivity, TArguments> AddExecuteActivity<TActivity, TArguments>(
            Action<IRegistrationContext, IExecuteActivityConfigurator<TActivity, TArguments>> configure = null)
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
            Action<IRegistrationContext, IExecuteActivityConfigurator<TActivity, TArguments>> configure = null)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class;

        /// <summary>
        /// Adds an activity (Courier), allowing configuration when it is configured on the endpoint.
        /// </summary>
        /// <param name="configureExecute">The execute configuration callback</param>
        /// <param name="configureCompensate">The compensate configuration callback</param>
        /// <typeparam name="TActivity">The activity type</typeparam>
        /// <typeparam name="TArguments">The argument type</typeparam>
        /// <typeparam name="TLog">The log type</typeparam>
        IActivityRegistrationConfigurator<TActivity, TArguments, TLog> AddActivity<TActivity, TArguments, TLog>(
            Action<IRegistrationContext, IExecuteActivityConfigurator<TActivity, TArguments>> configureExecute = null,
            Action<IRegistrationContext, ICompensateActivityConfigurator<TActivity, TLog>> configureCompensate = null)
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
            Action<IRegistrationContext, IExecuteActivityConfigurator<TActivity, TArguments>> configureExecute = null,
            Action<IRegistrationContext, ICompensateActivityConfigurator<TActivity, TLog>> configureCompensate = null)
            where TActivity : class, IActivity<TArguments, TLog>
            where TLog : class
            where TArguments : class;

        /// <summary>
        /// Adds an endpoint definition, which will to used for consumers, sagas, etc. that are on that same endpoint. If a consumer, etc.
        /// specifies an endpoint without a definition, the default endpoint definition is used if one cannot be resolved from the configuration
        /// service provider (via generic registration).
        /// </summary>
        /// <param name="endpointDefinition">The endpoint definition to add</param>
        void AddEndpoint(Type endpointDefinition);

        void AddEndpoint<TDefinition, T>(IRegistration registration, IEndpointSettings<IEndpointDefinition<T>> settings = null)
            where TDefinition : class, IEndpointDefinition<T>
            where T : class;

        /// <summary>
        /// Add a request client, for the request type, which uses the <see cref="ConsumeContext" /> if present, otherwise
        /// uses the <see cref="IBus" />. The request is published, unless an endpoint convention is specified for the
        /// request type.
        /// </summary>
        /// <param name="timeout">The request timeout</param>
        /// <typeparam name="T">The request message type</typeparam>
        void AddRequestClient<T>(RequestTimeout timeout = default)
            where T : class;

        /// <summary>
        /// Add a request client, for the request type, which uses the <see cref="ConsumeContext" /> if present, otherwise
        /// uses the <see cref="IBus" />.
        /// </summary>
        /// <param name="destinationAddress">The destination address for the request</param>
        /// <param name="timeout">The request timeout</param>
        /// <typeparam name="T">The request message type</typeparam>
        void AddRequestClient<T>(Uri destinationAddress, RequestTimeout timeout = default)
            where T : class;

        /// <summary>
        /// Add a request client, for the request type, which uses the <see cref="ConsumeContext" /> if present, otherwise
        /// uses the <see cref="IBus" />. The request is published, unless an endpoint convention is specified for the
        /// request type.
        /// </summary>
        /// <param name="requestType">The request message type</param>
        /// <param name="timeout">The request timeout</param>
        void AddRequestClient(Type requestType, RequestTimeout timeout = default);

        /// <summary>
        /// Add a request client, for the request type, which uses the <see cref="ConsumeContext" /> if present, otherwise
        /// uses the <see cref="IBus" />.
        /// </summary>
        /// <param name="requestType">The request message type</param>
        /// <param name="destinationAddress">The destination address for the request</param>
        /// <param name="timeout">The request timeout</param>
        void AddRequestClient(Type requestType, Uri destinationAddress, RequestTimeout timeout = default);

        /// <summary>
        /// Sets the default request timeout for this bus instance, used by the client factory to create request clients
        /// </summary>
        /// <param name="timeout"></param>
        void SetDefaultRequestTimeout(RequestTimeout timeout);

        /// <summary>
        /// Sets the default request timeout for this bus instance, used by the client factory to create request clients
        /// </summary>
        /// <param name="d">days</param>
        /// <param name="h">hours</param>
        /// <param name="m">minutes</param>
        /// <param name="s">seconds</param>
        /// <param name="ms">milliseconds</param>
        void SetDefaultRequestTimeout(int? d = null, int? h = null, int? m = null, int? s = null, int? ms = null);

        /// <summary>
        /// Set the default endpoint name formatter used for endpoint names
        /// </summary>
        /// <param name="endpointNameFormatter"></param>
        void SetEndpointNameFormatter(IEndpointNameFormatter endpointNameFormatter);

        /// <summary>
        /// Add a saga repository for the specified saga type, by specifying the repository type via method chaining. Using this
        /// method alone does nothing, it should be followed with the appropriate repository configuration method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        ISagaRegistrationConfigurator<T> AddSagaRepository<T>()
            where T : class, ISaga;

        /// <summary>
        /// Specify a saga repository provider, that will be called when a saga is configured by type
        /// (without a specific generic call to AddSaga/AddSagaStateMachine)
        /// </summary>
        /// <param name="provider"></param>
        void SetSagaRepositoryProvider(ISagaRepositoryRegistrationProvider provider);

        /// <summary>
        /// Adds a future registration, along with an optional definition
        /// </summary>
        /// <param name="futureDefinitionType">The future definition type</param>
        /// <typeparam name="TFuture"></typeparam>
        IFutureRegistrationConfigurator<TFuture> AddFuture<TFuture>(Type futureDefinitionType = null)
            where TFuture : class, SagaStateMachine<FutureState>;
    }
}
