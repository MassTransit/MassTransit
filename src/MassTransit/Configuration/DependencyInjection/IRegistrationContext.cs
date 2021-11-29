namespace MassTransit
{
    using System;


    /// <summary>
    /// Registration contains the consumers and sagas that have been registered, allowing them to be configured on one or more
    /// receive endpoints.
    /// </summary>
    public interface IRegistrationContext :
        IServiceProvider
    {
        /// <summary>
        /// Configure a consumer on the receive endpoint
        /// </summary>
        /// <param name="consumerType">The consumer type</param>
        /// <param name="configurator"></param>
        void ConfigureConsumer(Type consumerType, IReceiveEndpointConfigurator configurator);

        /// <summary>
        /// Configure a consumer on the receive endpoint, with an optional configuration action
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        /// <typeparam name="T">The consumer type</typeparam>
        void ConfigureConsumer<T>(IReceiveEndpointConfigurator configurator, Action<IConsumerConfigurator<T>> configure = null)
            where T : class, IConsumer;

        /// <summary>
        /// Configure all registered consumers on the receive endpoint
        /// </summary>
        /// <param name="configurator"></param>
        void ConfigureConsumers(IReceiveEndpointConfigurator configurator);

        /// <summary>
        /// Configure a saga on the receive endpoint
        /// </summary>
        /// <param name="sagaType">The saga type</param>
        /// <param name="configurator"></param>
        void ConfigureSaga(Type sagaType, IReceiveEndpointConfigurator configurator);

        /// <summary>
        /// Configure a saga on the receive endpoint, with an optional configuration action
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        /// <typeparam name="T">The saga type</typeparam>
        void ConfigureSaga<T>(IReceiveEndpointConfigurator configurator, Action<ISagaConfigurator<T>> configure = null)
            where T : class, ISaga;

        /// <summary>
        /// Configure all registered sagas on the receive endpoint
        /// </summary>
        /// <param name="configurator"></param>
        void ConfigureSagas(IReceiveEndpointConfigurator configurator);

        /// <summary>
        /// Configure the specified execute activity type
        /// </summary>
        /// <param name="activityType"></param>
        /// <param name="configurator"></param>
        void ConfigureExecuteActivity(Type activityType, IReceiveEndpointConfigurator configurator);

        /// <summary>
        /// Configure the specified activity type
        /// </summary>
        /// <param name="activityType"></param>
        /// <param name="executeEndpointConfigurator">The configurator for the execute activity endpoint</param>
        /// <param name="compensateEndpointConfigurator">The configurator for the compensate activity endpoint</param>
        void ConfigureActivity(Type activityType, IReceiveEndpointConfigurator executeEndpointConfigurator,
            IReceiveEndpointConfigurator compensateEndpointConfigurator);

        /// <summary>
        /// Configure the specified activity type
        /// </summary>
        /// <param name="activityType"></param>
        /// <param name="executeEndpointConfigurator">The configurator for the execute activity endpoint</param>
        /// <param name="compensateAddress"></param>
        void ConfigureActivityExecute(Type activityType, IReceiveEndpointConfigurator executeEndpointConfigurator, Uri compensateAddress);

        /// <summary>
        /// Configure the specified activity type
        /// </summary>
        /// <param name="activityType"></param>
        /// <param name="compensateEndpointConfigurator">The configurator for the compensate activity endpoint</param>
        void ConfigureActivityCompensate(Type activityType, IReceiveEndpointConfigurator compensateEndpointConfigurator);

        /// <summary>
        /// Configure a future on the receive endpoint
        /// </summary>
        /// <param name="futureType">The saga type</param>
        /// <param name="configurator"></param>
        void ConfigureFuture(Type futureType, IReceiveEndpointConfigurator configurator);

        /// <summary>
        /// Configure a future on the receive endpoint, with an optional configuration action
        /// </summary>
        /// <param name="configurator"></param>
        /// <typeparam name="T">The saga type</typeparam>
        void ConfigureFuture<T>(IReceiveEndpointConfigurator configurator)
            where T : class, ISaga;
    }
}
